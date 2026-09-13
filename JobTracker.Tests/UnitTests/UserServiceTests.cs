using AutoMapper;
using JobTracker.Application.DTOs.AuthDtos.ChangePasswordDtos;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
using JobTracker.Application.DTOs.AuthDtos.RefreshTokenDtos;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.AuthDtos.UserDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using Moq;

namespace JobTracker.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock = new();

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsConflictException()
    {
        // Arrange

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync("test@example.com"))
            .ReturnsAsync(true);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act

        var action = async () =>
            await service.RegisterAsync(dto);

        // Assert

        await Assert.ThrowsAsync<ConflictException>(action);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsUserDto()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync("test@example.com"))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(x => x.Map<User>(It.IsAny<RegisterDto>()))
            .Returns(new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            });

        _passwordHasherMock
            .Setup(x => x.HashPassword("Password123!"))
            .Returns("hashed-password");

        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            });

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "Password123!"
        };


        // Act
        var result = await service.RegisterAsync(dto);


        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal("test@example.com", result.Email);

        _userRepositoryMock.Verify(
            x => x.CreateAsync(It.Is<User>(u =>
                u.Email == "test@example.com" &&
                u.PasswordHash == "hashed-password")),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponseDto()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "Password123!",
                "hashed-password"))
            .Returns(true);

        _jwtServiceMock
            .Setup(x => x.GenerateToken(
                1,
                "test@example.com"))
            .Returns("fake-jwt-token");

        var expiration = DateTime.UtcNow.AddMinutes(30);

        _jwtServiceMock
            .Setup(x => x.GetExpiration())
            .Returns(expiration);

        _refreshTokenServiceMock
            .Setup(x => x.GenerateToken())
            .Returns("fake-refresh-token");

        _refreshTokenServiceMock
            .Setup(x => x.HashToken("fake-refresh-token"))
            .Returns("fake-refresh-token-hash");
        var refreshTokenExpiration =
            DateTime.UtcNow.AddDays(7);

        _refreshTokenServiceMock
            .Setup(x => x.GetExpiration())
            .Returns(refreshTokenExpiration);

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        _mapperMock
            .Setup(x => x.Map<UserDto>(user))
            .Returns(userDto);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "fake-jwt-token",
            result.AccessToken);

        Assert.Equal(
            "fake-refresh-token",
            result.RefreshToken);

        Assert.Equal(
            expiration,
            result.ExpiresAt);

        Assert.NotNull(result.User);

        Assert.Equal(1, result.User.Id);
        Assert.Equal("Test", result.User.FirstName);
        Assert.Equal("User", result.User.LastName);
        Assert.Equal(
            "test@example.com",
            result.User.Email);

        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync("test@example.com"),
            Times.Once);

        _passwordHasherMock.Verify(
            x => x.VerifyPassword(
                "Password123!",
                "hashed-password"),
            Times.Once);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                1,
                "test@example.com"),
            Times.Once);

        _refreshTokenServiceMock.Verify(
            x => x.GenerateToken(),
            Times.Once);

        _refreshTokenServiceMock.Verify(
            x => x.HashToken("fake-refresh-token"),
            Times.Once);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<RefreshToken>(token =>
                    token.UserId == 1 &&
                    token.TokenHash == "fake-refresh-token-hash" &&
                    token.ExpiresAt == refreshTokenExpiration)),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ThrowsUnauthorizedException()
    {
        // Arrange

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync((User?)null);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act

        var action = async () =>
            await service.LoginAsync(dto);

        // Assert

        await Assert.ThrowsAsync<UnauthorizedException>(action);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ThrowsUnauthorizedException()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "WrongPassword!",
                "hashed-password"))
            .Returns(false);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword!"
        };

        // Act
        var action = async () =>
            await service.LoginAsync(dto);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(action);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<long>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_WithValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed-password"
        };

        var oldRefreshToken = new RefreshToken
        {
            Id = 10,
            UserId = 1,
            TokenHash = "old-refresh-token-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(5)
        };

        var newRefreshTokenExpiration =
            DateTime.UtcNow.AddDays(7);

        var accessTokenExpiration =
            DateTime.UtcNow.AddMinutes(15);

        _refreshTokenServiceMock
            .Setup(x => x.HashToken("old-refresh-token"))
            .Returns("old-refresh-token-hash");

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenHashAsync(
                "old-refresh-token-hash"))
            .ReturnsAsync(oldRefreshToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _jwtServiceMock
            .Setup(x => x.GenerateToken(
                1,
                "test@example.com"))
            .Returns("new-access-token");

        _jwtServiceMock
            .Setup(x => x.GetExpiration())
            .Returns(accessTokenExpiration);

        _refreshTokenServiceMock
            .Setup(x => x.GenerateToken())
            .Returns("new-refresh-token");

        _refreshTokenServiceMock
            .Setup(x => x.HashToken("new-refresh-token"))
            .Returns("new-refresh-token-hash");

        _refreshTokenServiceMock
            .Setup(x => x.GetExpiration())
            .Returns(newRefreshTokenExpiration);

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        _mapperMock
            .Setup(x => x.Map<UserDto>(user))
            .Returns(userDto);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "old-refresh-token"
        };

        // Act
        var result = await service.RefreshAsync(dto);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            "new-access-token",
            result.AccessToken);

        Assert.Equal(
            "new-refresh-token",
            result.RefreshToken);

        Assert.Equal(
            accessTokenExpiration,
            result.ExpiresAt);

        Assert.NotNull(result.User);

        Assert.Equal(
            1,
            result.User.Id);

        Assert.Equal(
            "test@example.com",
            result.User.Email);

        Assert.NotNull(oldRefreshToken.RevokedAt);

        Assert.Equal(
            "new-refresh-token-hash",
            oldRefreshToken.ReplacedByTokenHash);

        _refreshTokenRepositoryMock.Verify(
            x => x.UpdateAsync(oldRefreshToken),
            Times.Once);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<RefreshToken>(token =>
                    token.UserId == 1 &&
                    token.TokenHash == "new-refresh-token-hash" &&
                    token.ExpiresAt == newRefreshTokenExpiration)),
            Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_WhenTokenDoesNotExist_ThrowsUnauthorizedException()
    {
        // Arrange
        _refreshTokenServiceMock
            .Setup(x => x.HashToken("invalid-refresh-token"))
            .Returns("invalid-refresh-token-hash");

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenHashAsync(
                "invalid-refresh-token-hash"))
            .ReturnsAsync((RefreshToken?)null);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var action = async () =>
            await service.RefreshAsync(dto);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(action);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<long>(),
                It.IsAny<string>()),
            Times.Never);

        _refreshTokenServiceMock.Verify(
            x => x.GenerateToken(),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_WhenTokenIsRevoked_ThrowsUnauthorizedException()
    {
        // Arrange
        var revokedRefreshToken = new RefreshToken
        {
            Id = 10,
            UserId = 1,
            TokenHash = "revoked-token-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(5),
            RevokedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        _refreshTokenServiceMock
            .Setup(x => x.HashToken("revoked-refresh-token"))
            .Returns("revoked-token-hash");

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenHashAsync(
                "revoked-token-hash"))
            .ReturnsAsync(revokedRefreshToken);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "revoked-refresh-token"
        };

        // Act
        var action = async () =>
            await service.RefreshAsync(dto);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(action);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<long>(),
                It.IsAny<string>()),
            Times.Never);

        _refreshTokenServiceMock.Verify(
            x => x.GenerateToken(),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_WhenTokenIsExpired_ThrowsUnauthorizedException()
    {
        // Arrange
        var expiredRefreshToken = new RefreshToken
        {
            Id = 10,
            UserId = 1,
            TokenHash = "expired-token-hash",
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
        };

        _refreshTokenServiceMock
            .Setup(x => x.HashToken("expired-refresh-token"))
            .Returns("expired-token-hash");

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenHashAsync(
                "expired-token-hash"))
            .ReturnsAsync(expiredRefreshToken);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "expired-refresh-token"
        };

        // Act
        var action = async () =>
            await service.RefreshAsync(dto);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(action);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<long>(),
                It.IsAny<string>()),
            Times.Never);

        _refreshTokenServiceMock.Verify(
            x => x.GenerateToken(),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_WhenUserDoesNotExist_ThrowsUnauthorizedException()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Id = 10,
            UserId = 999,
            TokenHash = "valid-token-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _refreshTokenServiceMock
            .Setup(x => x.HashToken("valid-refresh-token"))
            .Returns("valid-token-hash");

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenHashAsync(
                "valid-token-hash"))
            .ReturnsAsync(refreshToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "valid-refresh-token"
        };

        // Act
        var action = async () =>
            await service.RefreshAsync(dto);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedException>(action);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<long>(),
                It.IsAny<string>()),
            Times.Never);

        _refreshTokenServiceMock.Verify(
            x => x.GenerateToken(),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);

        _refreshTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_UpdatesPassword()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "old-hashed-password"
        };

        _userContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "OldPassword123!",
                "old-hashed-password"))
            .Returns(true);

        _passwordHasherMock
            .Setup(x => x.HashPassword("NewPassword123!"))
            .Returns("new-hashed-password");

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        // Act
        await service.ChangePasswordAsync(dto);

        // Assert
        Assert.Equal(
            "new-hashed-password",
            user.PasswordHash);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserDoesNotExist_ThrowsUnauthorizedException()
    {
        // Arrange

        _userContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((User?)null);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        // Act

        var action = async () =>
            await service.ChangePasswordAsync(dto);

        // Assert

        await Assert.ThrowsAsync<UnauthorizedException>(action);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsInvalid_ThrowsUnauthorizedException()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "old-hashed-password"
        };

        _userContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "WrongPassword123!",
                "old-hashed-password"))
            .Returns(false);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        // Act

        var action = async () =>
            await service.ChangePasswordAsync(dto);

        // Assert

        await Assert.ThrowsAsync<UnauthorizedException>(action);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsInvalid_DoesNotUpdatePassword()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "old-hashed-password"
        };

        _userContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "WrongPassword123!",
                "old-hashed-password"))
            .Returns(false);

        var service = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _userContextMock.Object,
            _refreshTokenRepositoryMock.Object,
            _refreshTokenServiceMock.Object);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        // Act

        var action = async () =>
            await service.ChangePasswordAsync(dto);

        await Assert.ThrowsAsync<UnauthorizedException>(action);

        // Assert

        _passwordHasherMock.Verify(
            x => x.HashPassword("NewPassword123!"),
            Times.Never);

        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>()),
            Times.Never);
    }
}