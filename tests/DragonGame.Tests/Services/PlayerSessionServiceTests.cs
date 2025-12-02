using DragonGame.Data;
using DragonGame.Repositories;
using DragonGame.Models;
using DragonGame.Services;
using Moq;
using FluentAssertions;
using Microsoft.Identity.Client;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore.Migrations;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace DragonGame.Tests.Services
{
    public class PlayerSessionServiceTests
    {
        private readonly Mock<ILogger<PlayerSessionService>> _loggerMock; // Mock of dependency
        private readonly Mock<IPlayerSessionRepository> _sessionRepoMock; // Mock of dependency
        private readonly Mock<ICharacterRepository> _characterRepoMock; // Mock of dependency
        private readonly Mock<IChoiceHistoryService> _choiceHistoryServiceMock;

        private readonly PlayerSessionService _sut; // SUT = System Under Testing
        public PlayerSessionServiceTests()
        {
            // create and inject a mock repos for testing
            _loggerMock = new Mock<ILogger<PlayerSessionService>>();
            _sessionRepoMock = new Mock<IPlayerSessionRepository>();
            _characterRepoMock = new Mock<ICharacterRepository>();
            _choiceHistoryServiceMock = new Mock<IChoiceHistoryService>();

            _sut = new PlayerSessionService(_sessionRepoMock.Object, _characterRepoMock.Object, _choiceHistoryServiceMock.Object, _loggerMock.Object);
        }

        // HELPERS
        private List<Choice> defaultChoices = new List<Choice>
         {
                    new Choice
                    {
                        ChoiceId = 101,
                        Text = "Go to the right",
                        NextActNumber = 11
                    },
                    new Choice
                    {
                        ChoiceId = 102,
                        Text="Go to the left",
                        NextActNumber = 12
                    }
        };
        private PlayerSession CreateMockSession(List<Choice> choices, int sessionId = 1, int currentActNumber = 1)
        {
            var act = new Act
            {
                ActNumber = currentActNumber,
                StoryId = 1,
                Text = "You come across a path leading two ways. Which way will you go?",
                Choices = choices
            };

            return new PlayerSession
            {
                SessionId = sessionId,
                CharacterName = "Stålfinn",
                CharacterId = 1,
                StoryId = 1,
                CurrentActNumber = currentActNumber,
                CurrentAct = act,
                IsCompleted = false
            };
        }
        private PlayerSession CreateMockSeshWithNoCurrAct(int sessionId = 2)
        {
            return new PlayerSession
            {
                SessionId = sessionId,
                CharacterName = "Bob",
                CharacterId = 2,
                StoryId = 2,
                CurrentActNumber = 2,
                CurrentAct = null,
                IsCompleted = false
            };
        }

        // TESTS 

        [Fact]
        public async Task MoveToNextActAsync_SessionFound_MatchingChoice_SaveHistoryUpdateSession()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 1, 1);
            _sessionRepoMock.Setup(repo => repo.GetWithChoicesAsync(1)).ReturnsAsync(session);
            _sessionRepoMock.Setup(repo => repo.UpdateAsync(session)).Returns(Task.CompletedTask);
            _choiceHistoryServiceMock.Setup(repo => repo.AddChoiceAsync(It.IsAny<ChoiceHistory>())).Returns(Task.CompletedTask);


            // When
            var result = await _sut.MoveToNextActAsync(1, 11);

            // Then
            result.Should().NotBeNull();
            result!.SessionId.Should().Be(1);
            result.CurrentActNumber.Should().Be(11);
            result.IsCompleted.Should().BeFalse();
            _sessionRepoMock.Verify(repo => repo.GetWithChoicesAsync(1), Times.Once);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(session), Times.Once);
            _choiceHistoryServiceMock.Verify(repo => repo.AddChoiceAsync(It.Is<ChoiceHistory>(history => history.PlayerSessionId == 1 && history.ActNumber == 1 && history.ChoiceId == 101)), Times.Once);


        }
        [Fact]
        public async Task MoveToNextActAsync_NoSessionFound_ReturnNull()
        {
            // Given
            _sessionRepoMock.Setup(repo => repo.GetWithChoicesAsync(888)).ReturnsAsync((PlayerSession?)null);

            // When
            var result = await _sut.MoveToNextActAsync(888, 999);

            // Then
            result.Should().BeNull();
            _choiceHistoryServiceMock.Verify(service => service.AddChoiceAsync(It.IsAny<ChoiceHistory>()), Times.Never);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<PlayerSession>()), Times.Never);
        }
        [Fact]
        public async Task MoveToNextActAsync_NoActFound_ReturnNull()
        {
            // Given
            var session = CreateMockSeshWithNoCurrAct(2);
            _sessionRepoMock.Setup(repo => repo.GetWithChoicesAsync(2)).ReturnsAsync(session);

            // When
            var result = await _sut.MoveToNextActAsync(2, 3);

            // Then
            result.Should().BeNull();
            _choiceHistoryServiceMock.Verify(service => service.AddChoiceAsync(It.IsAny<ChoiceHistory>()), Times.Never);
            _sessionRepoMock.Verify(service => service.UpdateAsync(It.IsAny<PlayerSession>()), Times.Never);
        }
        [Fact]
        public async Task MoveToNextActAsync_NextActChoiceNotValid_ThrowsError()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 1, 1);
            _sessionRepoMock.Setup(repo => repo.GetWithChoicesAsync(1)).ReturnsAsync(session);


            // When
            var callResult = async () => await _sut.MoveToNextActAsync(1, 99);

            // Then
            await callResult.Should().ThrowAsync<InvalidOperationException>("Because choice does not match next act");
            _choiceHistoryServiceMock.Verify(service => service.AddChoiceAsync(It.IsAny<ChoiceHistory>()), Times.Never);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<PlayerSession>()), Times.Never);
        }
        [Fact]
        public async Task MoveToNextAcyAsyc_NextActZeroOrNegative_SessionCompleted()
        {
            // Given
            var endingChoices = new List<Choice>
            {
                new Choice
                {
                    ChoiceId = 666,
                    Text = "Go To Ending",
                    NextActNumber = 0,
                    ActId = 1,
                }
            };
            var session = CreateMockSession(endingChoices, 1);
            _sessionRepoMock.Setup(repo => repo.GetWithChoicesAsync(1)).ReturnsAsync(session);
            _sessionRepoMock.Setup(repo => repo.UpdateAsync(session)).Returns(Task.CompletedTask);
            _choiceHistoryServiceMock.Setup(service => service.AddChoiceAsync(It.IsAny<ChoiceHistory>())).Returns(Task.CompletedTask);

            // When
            var result = await _sut.MoveToNextActAsync(1, 0);

            // Then
            result.Should().NotBeNull();
            result!.SessionId.Should().Be(1);
            result.IsCompleted.Should().BeTrue();
            // result.CurrentActNumber.Should().Be(0);  // <---- CHECK IF SHOULD ACTUALLY CHANGE TO GIVEN "nextActNumber"
            _sessionRepoMock.Verify(repo => repo.GetWithChoicesAsync(1), Times.Once);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(session), Times.Once);
            _choiceHistoryServiceMock.Verify(service => service.AddChoiceAsync(It.Is<ChoiceHistory>(h => h.PlayerSessionId == 1 && h.ActNumber == 1 && h.ChoiceId == 666)), Times.Once);
        }

    }
}
