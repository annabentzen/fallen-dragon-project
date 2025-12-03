using DragonGame.Data;
using DragonGame.Dtos.Auth;
using DragonGame.Models;
using DragonGame.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SQLitePCL;

namespace DragonGame.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;

        public AuthServiceTests()
        {
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<AuthService>>();
        }

        /* ------------ HELPERS ------------ */
        private User CreateValidUser(string username = "testUser", string rawPassword = "testPass123")
        {
            return new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword),
                CreatedAt = DateTime.UtcNow,
            };
        }
        private AppDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: dbName).Options;
            return new AppDbContext(options);
        }
        private LoginDto CreateLogInDto(string username = "testUser", string password = "testPass123")
        {
            return new LoginDto
            {
                Username = username,
                Password = password
            };
        }


        /* ----------- TESTS ------------ */
        /* ---- CREATE ---- */
        /* ---- READ ---- */
        [Fact]
        public async Task LoginAsync_ValidInput_ReturnsAuthResponse()
        {
            // Given
            var dbName = nameof(LoginAsync_ValidInput_ReturnsAuthResponse);
            await using var context = CreateDbContext(dbName); // "await using" to ensure database is cleaned up after use
            var rawPassword = "KjellePassord!123";
            var testUser = CreateValidUser("KjelleNavn", rawPassword);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            _jwtServiceMock.Setup(s => s.GenerateToken(It.Is<User>(u => u.UserId == testUser.UserId))).Returns("test-jwt-token");
            var sut = new AuthService(context, _jwtServiceMock.Object, _loggerMock.Object);
            var dto = CreateLogInDto(testUser.Username, rawPassword);

            // When
            var result = await sut.LoginAsync(dto);

            // Then
            result.Should().NotBeNull();
            result!.UserId.Should().Be(testUser.UserId);
            result.Username.Should().Be(testUser.Username);
            result.Token.Should().Be("test-jwt-token");
            _jwtServiceMock.Verify(service => service.GenerateToken(It.Is<User>(u => u.UserId == testUser.UserId)), Times.Once);
        }
        /* ---- UPDATE ---- */
        /* ---- DELETE ---- */
    }
};