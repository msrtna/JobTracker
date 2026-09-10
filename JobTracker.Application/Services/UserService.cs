using AutoMapper;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.AuthDtos.UserDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository repository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
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
    }
}
