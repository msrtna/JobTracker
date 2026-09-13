using AutoMapper;
using JobTracker.Application.DTOs.AuthDtos.ChangePasswordDtos;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
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

        public UserService(
                IUserRepository repository,
                IMapper mapper,
                IPasswordHasher passwordHasher,
                IJwtService jwtService,
                IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _userContext = userContext;
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

            var token = _jwtService.GenerateToken(
                user.Id,
                user.Email);

            var expiration = _jwtService.GetExpiration();

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiration,
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
    }
}