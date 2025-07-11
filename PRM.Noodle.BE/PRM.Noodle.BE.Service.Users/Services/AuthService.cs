using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRM.Noodle.BE.Service.Users.Models;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Users.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !VerifyPassword(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (user.IsActive == false)
                throw new UnauthorizedAccessException("User account is deactivated.");

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token (you might want to create a RefreshToken entity)
            // For now, we'll just generate it

            var userDto = _mapper.Map<UserDto>(user);

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes())
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            var existingUsername = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == registerDto.Username);
            if (existingUsername != null)
                throw new InvalidOperationException("Username already exists.");

            var user = _mapper.Map<User>(registerDto);
            user.Password = HashPassword(registerDto.Password);
            user.Role = "customer"; // Default role
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var userDto = _mapper.Map<UserDto>(user);

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes())
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // In a real implementation, you should validate the refresh token
            // against stored tokens in the database
            throw new NotImplementedException("Refresh token validation not implemented. Store refresh tokens in database.");
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            // In a real implementation, you should remove the refresh token from database
            throw new NotImplementedException("Token revocation not implemented. Remove refresh tokens from database.");
        }

        public async Task<UserDto> GetProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            // Check if username is being changed and if it's already taken
            if (!string.IsNullOrWhiteSpace(updateProfileDto.Username) &&
                updateProfileDto.Username != user.Username)
            {
                var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == updateProfileDto.Username);
                if (existingUser != null)
                    throw new InvalidOperationException("Username already exists.");
            }

            _mapper.Map(updateProfileDto, user);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.Password))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.Password = HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "your-secret-key");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "customer")
                }),
                Expires = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "noodle-api",
                Audience = _configuration["Jwt:Audience"] ?? "noodle-client"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private int GetJwtExpirationMinutes()
        {
            return int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        }
    }
}
