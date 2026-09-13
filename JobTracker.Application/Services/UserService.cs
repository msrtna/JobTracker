using AutoMapper;
using JobTracker.Application.DTOs.AuthDtos.ChangePasswordDtos;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
using JobTracker.Application.DTOs.AuthDtos.RefreshTokenDtos;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.AuthDtos.UserDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IUserContext _userContext;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenService _refreshTokenService;

        public UserService(
                IUserRepository repository,
                IMapper mapper,
                IPasswordHasher passwordHasher,
                IJwtService jwtService,
                IUserContext userContext,
                IRefreshTokenRepository refreshTokenRepository,
                IRefreshTokenService refreshTokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _userContext = userContext;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenService = refreshTokenService;
        }


        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            if (await _repository.ExistsByEmailAsync(dto.Email))
            {
                throw new ConflictException(
                    $"User with email '{dto.Email}' already exists.");
            }

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            await _repository.CreateAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            var isPasswordValid =
                _passwordHasher.VerifyPassword(
                    dto.Password,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            var accessToken = _jwtService.GenerateToken(
                user.Id,
                user.Email);

            var accessTokenExpiration =
                _jwtService.GetExpiration();

            var refreshToken =
                _refreshTokenService.GenerateToken();

            var refreshTokenHash =
                _refreshTokenService.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = _refreshTokenService.GetExpiration()
            };

            await _refreshTokenRepository.AddAsync(
                refreshTokenEntity);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = accessTokenExpiration,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userId = _userContext.UserId;

            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedException(
                    "User not found.");
            }

            var isCurrentPasswordValid =
                _passwordHasher.VerifyPassword(
                    dto.CurrentPassword,
                    user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new UnauthorizedException(
                    "Current password is incorrect.");
            }

            user.PasswordHash =
                _passwordHasher.HashPassword(dto.NewPassword);

            await _repository.UpdateAsync(user);
        }

        public async Task<LoginResponseDto> RefreshAsync(RefreshTokenRequestDto dto)
        {
            var tokenHash =
                _refreshTokenService.HashToken(
                    dto.RefreshToken);

            var refreshToken =
                await _refreshTokenRepository
                    .GetByTokenHashAsync(tokenHash);

            if (refreshToken == null)
            {
                throw new UnauthorizedException(
                    "Invalid refresh token.");
            }

            if (refreshToken.RevokedAt.HasValue)
            {
                throw new UnauthorizedException(
                    "Refresh token has been revoked.");
            }

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException(
                    "Refresh token has expired.");
            }

            var user =
                await _repository
                    .GetByIdAsync(refreshToken.UserId);

            if (user == null)
            {
                throw new UnauthorizedException(
                    "User not found.");
            }

            var newAccessToken =
                _jwtService.GenerateToken(
                    user.Id,
                    user.Email);

            var newAccessTokenExpiration =
                _jwtService.GetExpiration();

            var newRefreshToken =
                _refreshTokenService.GenerateToken();

            var newRefreshTokenHash =
                _refreshTokenService.HashToken(
                    newRefreshToken);

            refreshToken.RevokedAt =
                DateTime.UtcNow;

            refreshToken.ReplacedByTokenHash =
                newRefreshTokenHash;

            await _refreshTokenRepository
                .UpdateAsync(refreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenHash,
                ExpiresAt =
                    _refreshTokenService.GetExpiration()
            };

            await _refreshTokenRepository
                .AddAsync(newRefreshTokenEntity);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = newAccessTokenExpiration,
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}