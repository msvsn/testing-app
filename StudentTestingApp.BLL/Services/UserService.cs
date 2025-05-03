using StudentTestingApp.BLL.DTOs;
using StudentTestingApp.BLL.Interfaces;
using StudentTestingApp.DAL.Interfaces;
using StudentTestingApp.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;

namespace StudentTestingApp.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<UserViewDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserViewDto>>(users);
            return userDtos.OrderBy(u => u.Name);
        }

        public async Task<UserViewDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserViewDto>(user);
        }
    }
}
