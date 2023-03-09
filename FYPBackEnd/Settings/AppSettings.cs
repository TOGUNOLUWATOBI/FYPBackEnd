namespace FYPBackEnd.Settings
{
    public class AppSettings
    {
        public string JwtSecret { get; set; }
        public int JwtLifespan { get; set; }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
    }
}
