using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRM.Noodle.BE.Service.Users.Models;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var sortedUsers = users.OrderBy(u => u.UserId);
            return _mapper.Map<IEnumerable<UserDto>>(sortedUsers);
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if username already exists
            var existingUsername = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == createUserDto.Username);
            if (existingUsername != null)
                throw new InvalidOperationException("Username already exists.");

            // Check if email already exists
            var existingEmail = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == createUserDto.Email);
            if (existingEmail != null)
                throw new InvalidOperationException("Email already exists.");

            var user = _mapper.Map<User>(createUserDto);

            // Hash the password using BCrypt
            user.Password = HashPassword(createUserDto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            // Set default role if not provided
            if (string.IsNullOrEmpty(user.Role))
                user.Role = "customer";

            // Set default IsActive if not provided
            if (user.IsActive == null)
                user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return null;

            // Check if username is being changed and if it already exists
            if (!string.IsNullOrWhiteSpace(updateUserDto.Username) &&
                updateUserDto.Username != user.Username)
            {
                var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == updateUserDto.Username);
                if (existingUser != null)
                    throw new InvalidOperationException("Username already exists.");
            }

            // Check if email is being changed and if it already exists
            if (!string.IsNullOrWhiteSpace(updateUserDto.Email) &&
                updateUserDto.Email != user.Email)
            {
                var existingUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == updateUserDto.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("Email already exists.");
            }

            _mapper.Map(updateUserDto, user);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Soft delete - set IsActive to false
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> RestoreUserAsync(int userId)
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

        public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.IsActive == true);
            var sortedUsers = users.OrderBy(u => u.UserId);
            return _mapper.Map<IEnumerable<UserDto>>(sortedUsers);
        }

        public async Task<IEnumerable<UserDto>> GetInactiveUsersAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.IsActive == false);
            var sortedUsers = users.OrderBy(u => u.UserId);
            return _mapper.Map<IEnumerable<UserDto>>(sortedUsers);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user != null;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
