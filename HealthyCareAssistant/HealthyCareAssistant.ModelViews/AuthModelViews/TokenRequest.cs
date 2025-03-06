using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.AuthModelViews
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string refreshToken { get; set; }
    }
}
