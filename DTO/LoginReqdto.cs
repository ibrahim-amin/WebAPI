using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DTO
{
    public class LoginReqdto
    {
        public string Username { set; get; }
        public string Password { set; get; }
        public string PasswordKey { set; get; }
    }
}
