using AutoMapper;
using BCrypt.Net;
using StudentTestingApp.BLL.DTOs;
using StudentTestingApp.BLL.Interfaces;
using StudentTestingApp.Core.Models;
using StudentTestingApp.DAL.Interfaces;
using System.Threading.Tasks;
using System;
using StudentTestingApp.BLL.Exceptions;
using StudentTestingApp.BLL.Infrastructure;

namespace StudentTestingApp.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceResult<UserViewDto>> LoginAsync(UserLoginDto loginDto)
        {
            if (loginDto == null)
            {
                return ServiceResult<UserViewDto>.Failure("Login data cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return ServiceResult<UserViewDto>.Failure("Username and password cannot be empty.");
            }

            try
            {
                var users = await _unitOfWork.Users.FindAsync(u => u.Username.Equals(loginDto.Username, StringComparison.OrdinalIgnoreCase));
                var user = users.FirstOrDefault();

                if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return ServiceResult<UserViewDto>.Failure("Invalid username or password."); 
                }

                var userViewDto = _mapper.Map<UserViewDto>(user);
                return ServiceResult<UserViewDto>.Ok(userViewDto); 
            }
            catch (Exception ex) 
            {
                return ServiceResult<UserViewDto>.Failure($"An unexpected error occurred during login: {ex.Message}");
            }
        }
        
        public async Task<ServiceResult<UserViewDto>> RegisterAsync(UserRegisterDto userDto)
        {
             if (userDto == null)
             {
                 return ServiceResult<UserViewDto>.Failure("Registration data cannot be null.");
             }

            var validationErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(userDto.Name)) validationErrors.Add("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(userDto.Username)) validationErrors.Add("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(userDto.Password)) validationErrors.Add("Password cannot be empty.");

            if (validationErrors.Any())
            {
                return ServiceResult<UserViewDto>.Failure(validationErrors);
            }

            var existingUsers = await _unitOfWork.Users.FindAsync(u => u.Username.Equals(userDto.Username, StringComparison.OrdinalIgnoreCase));
            if (existingUsers.Any())
            {
                return ServiceResult<UserViewDto>.Failure($"Username '{userDto.Username}' is already taken.");
            }

            try
            {
                var newUser = _mapper.Map<User>(userDto);
                newUser.PasswordHash = PasswordHasher.HashPassword(userDto.Password);
                newUser.Id = Guid.NewGuid();

                await _unitOfWork.Users.AddAsync(newUser);
                var changes = await _unitOfWork.SaveChangesAsync();

                if (changes <= 0)
                {
                    return ServiceResult<UserViewDto>.Failure("Failed to save the new user due to a data access error.");
                }

                var userViewDto = _mapper.Map<UserViewDto>(newUser);
                return ServiceResult<UserViewDto>.Ok(userViewDto);
            }
            catch (Exception)
            {
                 return ServiceResult<UserViewDto>.Failure("An unexpected error occurred during registration.");
            }
        }
    }
}
