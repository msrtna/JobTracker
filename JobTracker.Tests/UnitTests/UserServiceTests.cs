using AutoMapper;
using JobTracker.Application.DTOs.AuthDtos.ChangePasswordDtos;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
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
            _userContextMock.Object);

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
            _userContextMock.Object);

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
            .Setup(x => x.GenerateToken(1, "test@example.com"))
            .Returns("fake-jwt-token");

        var expiration = DateTime.UtcNow.AddMinutes(30);

        _jwtServiceMock
            .Setup(x => x.GetExpiration())
            .Returns(expiration);

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
            _userContextMock.Object);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };


        // Act
        var result = await service.LoginAsync(dto);


        // Assert
        Assert.NotNull(result);

        Assert.Equal("fake-jwt-token", result.Token);

        Assert.Equal(expiration, result.ExpiresAt);

        Assert.NotNull(result.User);

        Assert.Equal(1, result.User.Id);
        Assert.Equal("Test", result.User.FirstName);
        Assert.Equal("User", result.User.LastName);
        Assert.Equal("test@example.com", result.User.Email);

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
            _userContextMock.Object);

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
            _userContextMock.Object);

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
            _userContextMock.Object);

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
            _userContextMock.Object);

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
            _userContextMock.Object);

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
            _userContextMock.Object);

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