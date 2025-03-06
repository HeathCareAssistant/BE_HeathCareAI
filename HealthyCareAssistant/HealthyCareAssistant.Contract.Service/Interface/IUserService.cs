using HealthyCareAssistant.Contact.Repo.Entity;
<<<<<<< HEAD
using HealthyCareAssistant.Core.Base;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using HealthyCareAssistant.ModelViews.DrugModelViews;
=======
using HealthyCareAssistant.ModelViews.AuthModelViews;
>>>>>>> main
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
<<<<<<< HEAD
        Task<(IEnumerable<UserModelView> users, int totalElement, int totalPage)> GetAllUsersPaginatedAsync(int page, int pageSize);
        Task<UserModelView> GetUserByIdAsync(int userId);
        Task<string> CreateUserAsync(UserCreateRequest request);
        Task<string> UpdateUserAsync(int userId, UserUpdateRequest request);
        Task<string> DeleteUserAsync(int userId);
        Task<IEnumerable<UserModelView>> SearchUsersAsync(string keyword);

=======
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordModel model);
        Task<UserModelView?> GetUserByIdAsync(string userId);
        Task<bool> ValidatePasswordAsync(User user, string password);
>>>>>>> main
    }

}
