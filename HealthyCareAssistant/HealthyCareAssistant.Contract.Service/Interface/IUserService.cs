using HealthyCareAssistant.ModelViews.AuthModelViews;
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
    }
}
