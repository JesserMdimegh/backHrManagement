using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public string Destination { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }

        // 1-to-many with Utilisateur (notifications sent to users)
        public List<User> Users { get; set; } = new List<User>();
    }
}