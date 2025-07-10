namespace ProjectAether.Objects.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginRequest()
        {
            Username = "";
            Password = "";
        }
    }
}
