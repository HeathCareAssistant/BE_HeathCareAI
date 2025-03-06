using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthyCareAssistant.Contact.Repo.Entity;

namespace HealthyCareAssistant.ModelViews.UserModelViews
{
    public class UserModelView
    {
        public object UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public object PhoneNumber { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public object UpdatedAt { get; set; }
        public string Fcmtoken { get; set; }

        public UserModelView(User user)
        {
            UserId = user.UserId;
            Name = user.Name;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            Role = user.Role?.RoleName ?? "User"; // Lấy Role nếu có
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;
            Fcmtoken = user.Fcmtoken;
        }
        public UserModelView() { }
    }
}
