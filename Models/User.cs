namespace DatabaseConnectie.Models
{
    public class User
    {
        public string? profile_picture { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string new_username { get; set; }

    }
}
