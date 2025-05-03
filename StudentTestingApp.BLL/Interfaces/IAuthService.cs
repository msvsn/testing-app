using StudentTestingApp.BLL.DTOs;
using System.Threading.Tasks;
using StudentTestingApp.BLL.Infrastructure;

namespace StudentTestingApp.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<UserViewDto>> RegisterAsync(UserRegisterDto userDto);
        Task<ServiceResult<UserViewDto>> LoginAsync(UserLoginDto loginDto);
    }
}
