namespace autenticacion.Models
{
    public class TokenSettings
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresIn { get; set; }
    }
}
