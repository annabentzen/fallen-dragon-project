using DragonGame.Repositories;
using DragonGame.Models;
using DragonGame.Services;
using DragonGame.Dtos;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace DragonGame.Tests.Services
{
    public class PlayerSessionServiceTests
    {
        // MockS of dependencies
        private readonly Mock<ILogger<PlayerSessionService>> _loggerMock;
        private readonly Mock<IPlayerSessionRepository> _sessionRepoMock;
        private readonly Mock<ICharacterRepository> _characterRepoMock;
        private readonly Mock<IChoiceHistoryService> _choiceHistoryServiceMock;

        private readonly PlayerSessionService _sut; // SUT = System Under Testing
        public PlayerSessionServiceTests()
        {
            // create and inject a mocks
            _loggerMock = new Mock<ILogger<PlayerSessionService>>();
            _sessionRepoMock = new Mock<IPlayerSessionRepository>();
            _characterRepoMock = new Mock<ICharacterRepository>();
            _choiceHistoryServiceMock = new Mock<IChoiceHistoryService>();

            _sut = new PlayerSessionService(_sessionRepoMock.Object, _characterRepoMock.Object, _choiceHistoryServiceMock.Object, _loggerMock.Object);
        }

        /* ------------ HELPERS ------------ */
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
        private PlayerSession CreateMockSession(List<Choice> choices, int sessionId = 1, int currentActNumber = 1, int storyId = 1, Character? character = null, int userId = 1)
        {
            var testAct = new Act
            {
                ActId = 1,
                ActNumber = currentActNumber,
                StoryId = storyId,
                Text = "You come across a path leading two ways. Which way will you go?",
                IsEnding = false,
                Story = new Story { },
                Choices = choices
            };
            var testChar = character ?? new Character
            {
                Id = 1,
                Head = "mage2-head.png",
                Body = "mage2-body.png",
                PoseId = 2
            };

            return new PlayerSession
            {
                SessionId = sessionId,
                CharacterName = "Stålfinn",
                UserId = userId,
                CharacterId = testChar.Id,
                StoryId = storyId,
                CurrentActNumber = currentActNumber,
                CurrentAct = testAct,
                IsCompleted = false,
                Story = testAct.Story,
                User = new User { },
                Character = testChar
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
        private CreateSessionDto CreateSessionDtoSample()
        {
            return new CreateSessionDto
            {
                StoryId = 1,
                CharacterName = "TesteMannen",
                Character = new CharacterDto
                {
                    Head = "mage2-head.png",
                    Body = "mage2-body.png",
                    PoseId = 2
                }
            };
        }

        /* ------------ TESTS ------------ */

        /* ---- CREATE ---- */
        [Fact]
        public async Task CreateSessionAsync_ValidInput_CreateCharAndSesh()
        {
            // Given
            var dto = CreateSessionDtoSample();
            var userId = 33;
            _characterRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Character>())).Returns(Task.CompletedTask);
            _sessionRepoMock.Setup(repo => repo.AddAsync(It.IsAny<PlayerSession>())).Returns(Task.CompletedTask);

            // When
            var result = await _sut.CreateSessionAsync(dto, userId);

            // Then (results exist, and are correctly matched)
            result.Should().NotBeNull();
            result.Character.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.StoryId.Should().Be(dto.StoryId);
            result.CurrentActNumber.Should().Be(1);
            result.IsCompleted.Should().BeFalse();
            result.CharacterName.Should().Be(dto.CharacterName);
            result.Character.Head.Should().Be(dto.Character.Head);
            result.Character.Body.Should().Be(dto.Character.Body);
            result.Character.PoseId.Should().Be(dto.Character.PoseId);
            result.CharacterId.Should().Be(result.Character.Id);
            _characterRepoMock.Verify(repo => repo.AddAsync(It.Is<Character>(c => c.Head == dto.Character.Head && c.Body == dto.Character.Body && c.PoseId == dto.Character.PoseId)), Times.Once);
        }
        [Fact]
        public async Task CreateSessionAsync_CharRepoThrows_Propagate()
        {
            // Given
            var dto = CreateSessionDtoSample();
            var userId = 34;
            _characterRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Character>())).ThrowsAsync(new Exception("character repository fails"));

            // When
            var act = async () => await _sut.CreateSessionAsync(dto, userId);

            // Then
            await act.Should().ThrowAsync<Exception>().WithMessage("character repository fails");
            _characterRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Character>()), Times.Once);
            _sessionRepoMock.Verify(repo => repo.AddAsync(It.IsAny<PlayerSession>()), Times.Never);
        }
        [Fact]
        public async Task CreateSessionAsync_SessionRepoThrows_Propagate()
        {
            // Given
            var dto = CreateSessionDtoSample();
            var userId = 33;
            _characterRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Character>())).Returns(Task.CompletedTask);
            _sessionRepoMock.Setup(repo => repo.AddAsync(It.IsAny<PlayerSession>())).ThrowsAsync(new Exception("Session Repo Failed"));

            // When
            var act = async () => await _sut.CreateSessionAsync(dto, userId);

            // Then
            await act.Should().ThrowAsync<Exception>().WithMessage("Session Repo Failed");
            _characterRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Character>()), Times.Once);
            _sessionRepoMock.Verify(repo => repo.AddAsync(It.IsAny<PlayerSession>()), Times.Once);
        }


        /* ---- READ ---- */
        [Fact]
        public async Task GetSessionAsync_SessionExist_ReturnSeshWithChar()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 1, 2);
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(1)).ReturnsAsync(session);

            // When
            var result = await _sut.GetSessionAsync(1);

            // Then
            result.Should().NotBeNull();
            result!.SessionId.Should().Be(1);
            result.CurrentActNumber.Should().Be(2);
            result.Character.Should().NotBeNull();
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(1), Times.Once);
        }
        [Fact]
        public async Task GetSessionAsync_NoSeshExist_ReturnsNull()
        {
            // Given
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(99)).ReturnsAsync((PlayerSession?)null);

            // When
            var result = await _sut.GetSessionAsync(99);

            // Then
            result.Should().BeNull();
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(99), Times.Once);
        }

        [Fact]
        public async Task GetSessionDtoAsync_ValidSessionExists_ReturnsMappedDto()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 7, 3, 4, null, 9);
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(7)).ReturnsAsync(session);

            // When
            var result = await _sut.GetSessionDtoAsync(7);

            // Then
            result.Should().NotBeNull();
            result!.SessionId.Should().Be(7);
            result.UserId.Should().Be(9);
            result.CharacterName.Should().Be("Stålfinn");
            result.CharacterId.Should().Be(1);
            result.Head.Should().Be("mage2-head.png");
            result.Body.Should().Be("mage2-body.png");
            result.PoseId.Should().Be(2);
            result.StoryId.Should().Be(4);
            result.CurrentActNumber.Should().Be(3);
            result.IsCompleted.Should().BeFalse();
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(7), Times.Once);

        }
        [Fact]
        public async Task GetSessionDtoAsync_NoSession_ReturnsNull()
        {
            // Given
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(78)).ReturnsAsync((PlayerSession?)null);

            // When
            var result = await _sut.GetSessionDtoAsync(78);

            // Then
            result.Should().BeNull();
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(78), Times.Once);
        }
        [Fact]
        public async Task GetSessionDtoAsync_SeshExistButNoChar_ReturnsNull()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 404);
            session.Character = null!;
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(404)).ReturnsAsync(session);

            // When
            var result = await _sut.GetSessionDtoAsync(404);

            // Then
            result.Should().BeNull();
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(404), Times.Once);
        }

        [Fact]
        public async Task GetCharacterForSessionAsync_ValidSeshCharExists_ReturnsChar()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 5);
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(5)).ReturnsAsync(session);

            // When
            var result = await _sut.GetCharacterForSessionAsync(5);

            // Then
            result.Should().NotBeNull();
            result.Should().BeSameAs(session.Character);
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(5), Times.Once);
        }
        [Fact]
        public async Task GetCharacterForSessionAsync_SeshOrCharMissing_ReturnsNull()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 47);
            session.Character = null!;
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(47)).ReturnsAsync(session);

            // When
            var result = await _sut.GetCharacterForSessionAsync(47);

            // Then
            result.Should().BeNull();
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(47), Times.Once);
        }

        /* ---- UPDATE ---- */
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
        public async Task MoveToNextActAsyc_NextActZeroOrNegative_SessionCompleted()
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
            _sessionRepoMock.Verify(repo => repo.GetWithChoicesAsync(1), Times.Once);
            _sessionRepoMock.Verify(repo => repo.UpdateAsync(session), Times.Once);
            _choiceHistoryServiceMock.Verify(service => service.AddChoiceAsync(It.Is<ChoiceHistory>(h => h.PlayerSessionId == 1 && h.ActNumber == 1 && h.ChoiceId == 666)), Times.Once);
        }

        [Fact]
        public async Task UpdateCharacterAsync_ValidSeshAndChar_ReturnSeshWithUpdatedChar()
        {
            // Given
            var existingChar = new Character
            {
                Id = 19,
                Body = "original-body.png",
                Head = "original-head.png",
                PoseId = 2
            };
            var updatedChar = new Character
            {
                Body = "new-body.png",
                Head = "new-head.png",
                PoseId = 1
            };
            var session = CreateMockSession(defaultChoices, 1, 1, 1, existingChar, 21);
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(1)).ReturnsAsync(session);
            _characterRepoMock.Setup(repo => repo.UpdateAsync(existingChar)).Returns(Task.CompletedTask);

            // When
            var result = await _sut.UpdateCharacterAsync(1, updatedChar);

            // Then
            result.Should().NotBeNull();
            result.Should().BeSameAs(session);
            result!.Character.Should().NotBeNull();
            result.Character.Id.Should().Be(19);
            result.Character.Head.Should().Be("new-head.png");
            result.Character.Body.Should().Be("new-body.png");
            result.Character.PoseId.Should().Be(1);
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(1), Times.Once);
            _characterRepoMock.Verify(repo => repo.UpdateAsync(existingChar), Times.Once);
        }
        [Fact]
        public async Task UpdateCharacterAsync_MissingSeshOrChar_ThrowsException()
        {
            // Given
            var session = CreateMockSession(defaultChoices, 44);
            session.Character = null!;
            var updatedChar = new Character
            {
                Body = "new-body.png",
                Head = "new-head.png",
                PoseId = 1
            };
            _sessionRepoMock.Setup(repo => repo.GetWithCharacterAsync(44)).ReturnsAsync((PlayerSession?)null);

            // When
            var act = async () => await _sut.UpdateCharacterAsync(44, updatedChar);

            // Then
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Session 44 or character not found");
            _sessionRepoMock.Verify(repo => repo.GetWithCharacterAsync(44), Times.Once);
            _characterRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Character>()), Times.Never);
        }


        /* ---- DELETE ---- */

    }
}
