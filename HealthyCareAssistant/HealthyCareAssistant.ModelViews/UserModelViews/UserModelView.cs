using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using HealthyCareAssistant.Contact.Repo.Entity;
>>>>>>> main

namespace HealthyCareAssistant.ModelViews.UserModelViews
{
    public class UserModelView
    {
<<<<<<< HEAD
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Fcmtoken { get; set; }
        public DateTime? CreatedAt { get; set; }
=======
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
>>>>>>> main
    }
}
