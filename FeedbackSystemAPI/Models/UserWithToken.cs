using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackSystemAPI.Models
{
    public class UserWithToken : User
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        
        public UserWithToken(User user)
        {
            this.UserId = user.UserId;
            this.Email = user.Email;
            this.RoleId = user.RoleId;
            this.Name = user.Name;
            this.PhoneNumber = user.PhoneNumber;
           
    }
        public UserWithToken()
        {

        }
    }
}
