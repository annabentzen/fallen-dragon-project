using DragonGame.Data;
using DragonGame.Repositories;
using DragonGame.Models;
using DragonGame.Services;
using Moq;
using FluentAssertions;
using Microsoft.Identity.Client;

namespace DragonGame.Tests.Services
{
    public class StoryServiceTests
    {
        private readonly Mock<IStoryRepository> _storyRepoMock; // Mock of dependency
        private readonly Mock<IPlayerSessionRepository> _sessionRepoMock; // Mock of dependency
        private readonly Mock<ICharacterRepository> _characterRepoMock; // Mock of dependency
        private readonly Mock<IChoiceHistoryService> _choiceHistoryServiceMock;

        private readonly StoryService _sut; // SUT = System Under Testing
        public StoryServiceTests()
        {
            // create and inject a mock repos for testing
            _storyRepoMock = new Mock<IStoryRepository>();
            _sessionRepoMock = new Mock<IPlayerSessionRepository>();
            _characterRepoMock = new Mock<ICharacterRepository>();
            _choiceHistoryServiceMock = new Mock<IChoiceHistoryService>();
            AppDbContext contextStub = null!; // Has to be changed for methods that need DbContext.

            _sut = new StoryService(_sessionRepoMock.Object, _storyRepoMock.Object, _characterRepoMock.Object, contextStub, _choiceHistoryServiceMock.Object);
        }

        // HELPERS
        private PlayerSession CreateMockSession(int sessionId = 1, int currentActNumber = 1, int nextActNumber = 11)
        {
            var act = new Act
            {
                ActNumber = currentActNumber,
                StoryId = 1,
                Text = "You come across a path leading two ways. Which way will you go?",
                Choices = new List<Choice>
                {
                    new Choice
                    {
                        ChoiceId = 101,
                        Text = "Go to the right",
                        NextActNumber = nextActNumber
                    },
                    new Choice
                    {
                        ChoiceId = 102,
                        Text="Go to the left",
                        NextActNumber = 12
                    }
                }

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


        // TESTS 

        [Fact]
        public async Task MoveToNextActAsync_SessionFound_MatchingChoice_SaveHistoryUpdateSession()
        {
            // Given
            var session = CreateMockSession(1, 1, 11);
            _sessionRepoMock.Setup(repo => repo.GetSessionByIdWithChoicesAsync(1)).ReturnsAsync(session);
            _sessionRepoMock.Setup(repo => repo.UpdateAsync(session)).Returns(Task.CompletedTask);
            _sessionRepoMock.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);
            _choiceHistoryServiceMock.Setup(repo => repo.AddChoiceAsync(It.IsAny<ChoiceHistory>())).Returns(Task.CompletedTask);


            // When
            var result = await _sut.MoveToNextActAsync(1, 11);

            // Then
            result.Should().NotBeNull();
            result!.SessionId.Should().Be(1);
            result.CurrentActNumber.Should().Be(11);
            result.IsCompleted.Should().BeFalse();
            _sessionRepoMock.Verify(repo => repo.GetSessionByIdWithChoicesAsync(1), Times.Once);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(session), Times.Once);
            _sessionRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _choiceHistoryServiceMock.Verify(repo => repo.AddChoiceAsync(It.Is<ChoiceHistory>(history => history.PlayerSessionId == 1 && history.ActNumber == 1 && history.ChoiceId == 101)), Times.Once);


        }
        [Fact]
        public async Task MoveToNextActAsync_NoSessionFound_ReturnNull()
        {
            // Given
            _sessionRepoMock.Setup(repo => repo.GetSessionByIdWithChoicesAsync(888)).ReturnsAsync((PlayerSession?)null);

            // When
            var result = await _sut.MoveToNextActAsync(888, 999);

            // Then
            result.Should().BeNull();
            _choiceHistoryServiceMock.Verify(service => service.AddChoiceAsync(It.IsAny<ChoiceHistory>()), Times.Never);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<PlayerSession>()), Times.Never);
            _sessionRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task TestName()
        {
            // Given

            // When

            // Then
        }
    }
}
