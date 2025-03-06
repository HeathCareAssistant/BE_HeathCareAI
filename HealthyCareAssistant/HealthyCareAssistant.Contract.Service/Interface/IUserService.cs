using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.ModelViews.AuthModelViews;
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
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordModel model);
        Task<UserModelView?> GetUserByIdAsync(string userId);
        Task<bool> ValidatePasswordAsync(User user, string password);
    }

}
