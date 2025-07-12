using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectAether.Objects.Net._2._1.Standard.Models
{
    [Serializable]
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public RegisterRequest()
        {
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}
