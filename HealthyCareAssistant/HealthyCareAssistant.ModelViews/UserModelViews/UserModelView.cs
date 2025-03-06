using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.UserModelViews
{
    public class UserModelView
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Fcmtoken { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
