namespace Converter.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }


        public void UpdateModel(User user)
        {
            this.Login = user.Login == null ? this.Login : user.Login;
            this.Password = user.Password == null ? this.Password : user.Password;
            this.Role = user.Role == null ? this.Role : user.Role;
        }
    }
}