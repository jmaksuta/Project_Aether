namespace ProjectAether.Objects.Models
{
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
