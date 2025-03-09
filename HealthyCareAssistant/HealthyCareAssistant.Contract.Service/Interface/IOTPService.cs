using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IOTPService
    {
        string GenerateOtp();
        Task<bool> StoreOtpAsync(string email, string otp);
        Task<bool> ValidateOtpAsync(string email, string otp);
    }
}
