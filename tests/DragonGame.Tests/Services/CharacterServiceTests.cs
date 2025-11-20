using System.IO.Compression;
using System.Linq;
using DragonGame.Models;
using DragonGame.Repositories;
using FluentAssertions;
using Moq;
using server.Services;

namespace DragonGame.Tests.Services
{
    public class CharacterServiceTests
    {
        private readonly Mock<ICharacterRepository> _characterRepoMock; // Mock of dependency
        private readonly CharacterService _sut; // SUT = System Under Testing
        public CharacterServiceTests()
        {
            // create and inject a mock repo for testing
            _characterRepoMock = new Mock<ICharacterRepository>();
            _sut = new CharacterService(_characterRepoMock.Object);
        }

        // method to help create characters for tests quicker. MAX ID 3!!
        private Character CreateValidCharacterById(int Id, string? hair = null, string? face = null, string? outfit = null, int? pose = null)
        {
            if (Id is < 0 or > 3) throw new ArgumentException(nameof(Id), "ID cannot currently go higher than 3 or lower than 1");
            return new Character
            {
                Id = Id,
                Hair = hair ?? $"hair{Id}.png",
                Face = face ?? $"face{Id}.png",
                Outfit = outfit ?? $"clothing{Id}.png",
                PoseId = pose ?? Id,
            };
        }

        // CREATE
        [Fact]
        public async Task CreateAsync_ValidChar_CallsAddReturnsChar()
        {
            // Given
            var character = CreateValidCharacterById(1);
            _characterRepoMock.Setup(repo => repo.AddAsync(character)).Returns(Task.CompletedTask).Verifiable();

            // When
            var result = await _sut.CreateAsync(character);

            // Then
            result.Should().BeSameAs(character); // Verify what is sent in is actually what is added
            _characterRepoMock.Verify(repo => repo.AddAsync(character), Times.Once); // Verify "AddAsync" gets called exaclty once
        }
        [Fact]
        public async Task CreateAsync_WhenRepoThrows_PropagateException()
        {
            // Arrange
            var character = CreateValidCharacterById(1);
            _characterRepoMock.Setup(repo => repo.AddAsync(character)).ThrowsAsync(new Exception("Database Error"));

            // Act
            var act = async () => await _sut.CreateAsync(character);


            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database Error");
            _characterRepoMock.Verify(repo => repo.AddAsync(character), Times.Once);
        }

        // READ
        [Fact]
        public async Task GetAllAsync__WhenCharExists_ReturnsAll()
        {
            // Given
            var characters = new List<Character>
            {
                CreateValidCharacterById(1),
                CreateValidCharacterById(2)
            };
            _characterRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(characters);

            // When
            var result = await _sut.GetAllAsync();

            // Then
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(c => c.Id).Should().Contain([1, 2]);

            _characterRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
        [Fact]
        public async Task GetAllAsync_WhenNoCharsExist_ReturnsEmpty()
        {
            // Given
            _characterRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(Enumerable.Empty<Character>);

            // When
            var result = await _sut.GetAllAsync();

            // Then
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _characterRepoMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Exists_ReturnsChar()
        {
            // Given
            var character = CreateValidCharacterById(1);
            _characterRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(character);

            // When
            var result = await _sut.GetByIdAsync(1);

            // Then
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Hair.Should().Be(character.Hair);
            result.Face.Should().Be(character.Face);
            result.Outfit.Should().Be(character.Outfit);
            result.PoseId.Should().Be(character.PoseId);
            _characterRepoMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);

        }

        [Fact]
        public async Task GetByIdAsync_NotExist_ReturnsNull()
        {
            // Given
            _characterRepoMock.Setup(repo => repo.GetByIdAsync(9999)).ReturnsAsync((Character?)null);

            // When
            var result = await _sut.GetByIdAsync(9999);

            // Then
            result.Should().BeNull();
            _characterRepoMock.Verify(repo => repo.GetByIdAsync(9999), Times.Once);
        }

    }
}