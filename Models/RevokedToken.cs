namespace Back_HR.Models
{
    public class RevokedToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
