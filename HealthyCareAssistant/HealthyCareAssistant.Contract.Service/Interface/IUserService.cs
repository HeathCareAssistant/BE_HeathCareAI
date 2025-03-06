using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Core.Base;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using HealthyCareAssistant.ModelViews.UserModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModelViews model);
        Task<string> LoginAsync(LoginModelViews model);
        Task<(IEnumerable<UserModelView> users, int totalElement, int totalPage)> GetAllUsersPaginatedAsync(int page, int pageSize);
        Task<UserModelView> GetUserByIdAsync(int userId);
        Task<string> CreateUserAsync(UserCreateRequest request);
        Task<string> UpdateUserAsync(int userId, UserUpdateRequest request);
        Task<string> DeleteUserAsync(int userId);
        Task<IEnumerable<UserModelView>> SearchUsersAsync(string keyword);

    }
}
