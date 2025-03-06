using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.UserModelViews
{
    public class UserUpdateRequest
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Fcmtoken { get; set; }
    }
}
