using DragonGame.Data;
using DragonGame.Repositories;
using DragonGame.Models;
using DragonGame.Services;
using Moq;
using FluentAssertions;

namespace DragonGame.Tests.Services
{
    public class StoryServiceTests
    {
        private readonly Mock<IStoryRepository> _storyRepoMock; // Mock of dependency
        private readonly Mock<IPlayerSessionRepository> _sessionRepoMock; // Mock of dependency
        private readonly Mock<ICharacterRepository> _characterRepoMock; // Mock of dependency

        private readonly StoryService _sut; // SUT = System Under Testing
        public StoryServiceTests()
        {
            // create and inject a mock repos for testing
            _storyRepoMock = new Mock<IStoryRepository>();
            _sessionRepoMock = new Mock<IPlayerSessionRepository>();
            _characterRepoMock = new Mock<ICharacterRepository>();
            AppDbContext contextStub = null!; // Has to be changed for methods that need DbContext.

            _sut = new StoryService(_sessionRepoMock.Object, _storyRepoMock.Object, _characterRepoMock.Object, contextStub);
        }

        // HELPERS


        // TESTS 

        [Fact]
        public async Task MoveToNextActAsync_Positive()
        {
            // Given

            // When

            // Then
        }
    }
}