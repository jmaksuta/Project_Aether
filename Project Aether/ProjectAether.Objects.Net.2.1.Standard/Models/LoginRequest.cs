namespace ProjectAether.Objects.Net._2._1.Standard.Models
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
