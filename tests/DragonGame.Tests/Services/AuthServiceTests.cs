using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using BCrypt.Net;
using DragonGame.Data;
using DragonGame.Dtos.Auth;
using DragonGame.Models;
using DragonGame.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;

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
        private RegisterDto CreateRegisterDto(string username = "testUser", string password = "testPass123")
        {
            return new RegisterDto
            {
                Username = username,
                Password = password
            };
        }

        /* ----------- TESTS ------------ */
        /* ---- CREATE ---- */
        [Fact]
        public async Task RegisterAsync_ValidInput_AddsToContextAndReturnsAuthResponse()
        {
            // Given
            var dbName = nameof(RegisterAsync_ValidInput_AddsToContextAndReturnsAuthResponse);
            await using var context = CreateDbContext(dbName);

            var sut = new AuthService(context, _jwtServiceMock.Object, _loggerMock.Object);
            var dto = CreateRegisterDto("newUser", "initialPassword!123");

            _jwtServiceMock.Setup(s => s.GenerateToken(It.IsAny<User>())).Returns("test_register-token");

            // When
            var result = await sut.RegisterAsync(dto);
            var savedUser = await context.Users.SingleOrDefaultAsync(user => user.Username == dto.Username);

            // Then
            result.Should().NotBeNull();
            result!.Username.Should().Be(dto.Username);
            result.UserId.Should().BeGreaterThan(0);
            result.Token.Should().Be("test_register-token");

            savedUser.Should().NotBeNull();
            savedUser!.Username.Should().Be("newUser");
            BCrypt.Net.BCrypt.Verify("initialPassword!123", savedUser.PasswordHash).Should().BeTrue();
            _jwtServiceMock.Verify(s => s.GenerateToken(It.Is<User>(u => u.UserId == savedUser.UserId)), Times.Once);
        }
        [Fact]
        public async Task RegisterAsync_ExistingUser_ReturnsNull()
        {
            // Given
            var dbName = nameof(RegisterAsync_ExistingUser_ReturnsNull);
            await using var context = CreateDbContext(dbName);
            var existingUser = CreateValidUser("existingUser1", "existingPassword1");
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var sut = new AuthService(context, _jwtServiceMock.Object, _loggerMock.Object);
            var dto = CreateRegisterDto(existingUser.Username);

            // When
            var result = await sut.RegisterAsync(dto);
            var usersWithTakenName = await context.Users.Where(u => u.Username == existingUser.Username).ToListAsync();

            // Then
            result.Should().BeNull();
            usersWithTakenName.Should().HaveCount(1);
            _jwtServiceMock.Verify(s => s.GenerateToken(It.IsAny<User>()), Times.Never);
        }

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
        [Fact]
        public async Task LoginAsync_InvalidUser_ReturnsNull()
        {
            // Given
            var dbName = nameof(LoginAsync_InvalidUser_ReturnsNull);
            await using var context = CreateDbContext(dbName);

            var sut = new AuthService(context, _jwtServiceMock.Object, _loggerMock.Object);
            var dto = CreateLogInDto("doesNot", "exist");

            // When
            var result = await sut.LoginAsync(dto);

            // Then
            result.Should().BeNull();
            _jwtServiceMock.Verify(s => s.GenerateToken(It.IsAny<User>()), Times.Never);
        }
        [Fact]
        public async Task LoginAsync_ValidUserWrongPassword_ReturnsNull()
        {
            // Given
            var dbName = nameof(LoginAsync_ValidUserWrongPassword_ReturnsNull);
            await using var context = CreateDbContext(dbName); // "await using" to ensure database is cleaned up after use
            var correctPassword = "correctPassword!123";
            var testUser = CreateValidUser("testName", correctPassword);
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var sut = new AuthService(context, _jwtServiceMock.Object, _loggerMock.Object);
            var dto = CreateLogInDto(testUser.Username, "WrongPassword!");

            // When
            var result = await sut.LoginAsync(dto);

            // Then
            result.Should().BeNull();
            _jwtServiceMock.Verify(s => s.GenerateToken(It.IsAny<User>()), Times.Never);

        }

        /* ---- UPDATE ---- */
        /* ---- DELETE ---- */
    }
};