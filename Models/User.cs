using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Back_HR.Models
{
    public abstract class User : IdentityUser<Guid>
    {
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Telephone { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}