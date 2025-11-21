using DragonGame.Models;
using DragonGame.Repositories;
using FluentAssertions;
using Moq;
using DragonGame.Services;

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

        // HELPER - MAX ID 3!!
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

        // UPDATE
        [Fact]
        public async Task UpdateAsync_Exists_UpdateFieldsAndReturnsChar()
        {
            // Given
            var existingChar = CreateValidCharacterById(1);
            var updatedChar = CreateValidCharacterById(2);
            _characterRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingChar);
            _characterRepoMock.Setup(repo => repo.UpdateAsync(existingChar)).Returns(Task.CompletedTask);

            // When
            var result = await _sut.UpdateAsync(1, updatedChar);

            // Then
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Hair.Should().Be(updatedChar.Hair);
            result.Face.Should().Be(updatedChar.Face);
            result.Outfit.Should().Be(updatedChar.Outfit);
            result.PoseId.Should().Be(updatedChar.PoseId);
            _characterRepoMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _characterRepoMock.Verify(repo => repo.UpdateAsync(existingChar), Times.Once);
        }
        [Fact]
        public async Task UpdateAsync_NotExist_ReturnsNullAndNoUpdate()
        {
            // Given
            var updatedChar = CreateValidCharacterById(2);
            _characterRepoMock.Setup(repo => repo.GetByIdAsync(5678)).ReturnsAsync((Character?)null);

            // When
            var result = await _sut.UpdateAsync(5678, updatedChar);

            // Then
            result.Should().BeNull();
            _characterRepoMock.Verify(repo => repo.GetByIdAsync(5678), Times.Once);
            _characterRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Character>()), Times.Never);
        }


        // DELETE
        [Fact]
        public async Task DeleteAsync_Exist_DeleteCharReturnTrue()
        {
            // Given
            var existingChar = CreateValidCharacterById(1);
            _characterRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingChar);
            _characterRepoMock.Setup(repo => repo.DeleteAsync(existingChar.Id)).Returns(Task.CompletedTask);

            // When
            var result = await _sut.DeleteAsync(1);

            // Then
            result.Should().Be(true);
            _characterRepoMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _characterRepoMock.Verify(repo => repo.DeleteAsync(existingChar.Id), Times.Once);

        }
        [Fact]
        public async Task DeleteAsync_NotExist_DoNothingReturnFalse()
        {
            // Given
            _characterRepoMock.Setup(repo => repo.GetByIdAsync(8484)).ReturnsAsync((Character?)null);

            // When
            var result = await _sut.DeleteAsync(8484);

            // Then
            result.Should().BeFalse();
            _characterRepoMock.Verify(repo => repo.GetByIdAsync(8484), Times.Once);
            _characterRepoMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}