using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public abstract class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}