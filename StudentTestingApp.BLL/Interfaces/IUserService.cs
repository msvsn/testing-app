using StudentTestingApp.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentTestingApp.BLL.Interfaces
{
     public interface IUserService
     {
         Task<UserViewDto?> GetUserByIdAsync(Guid userId);
         Task<IEnumerable<UserViewDto>> GetAllUsersAsync();
     }
}
