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
        public string Name { get; set; }
        public string Email { get; set; }
        public object UserId { get; set; }
        public object PhoneNumber { get; set; }
        public string? Role { get; set; }
        public object CreatedAt { get; set; }
        public object UpdatedAt { get; set; }

        public UserModelView(User user)
        {
            Name = user.Name;
            Email = user.Email;
        }
    }
}
