using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackSystemAPI.Models
{
    public class UserInfo : User
    {
        public UserInfo(User user)
        {
            this.UserId = user.UserId;
            this.Email = user.Email;
            this.RoleId = user.RoleId;
            this.Name = user.Name;
            this.Role = user.Role;
            this.PhoneNumber = user.PhoneNumber;
        }
        public UserInfo()
        {

        }
    }
}
