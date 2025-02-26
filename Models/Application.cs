using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ApplicationDate { get; set; } = DateTime.Now; // When the application was submitted
        public string Status { get; set; } = "PENDING"; // E.g., PENDING, ACCEPTED, REJECTED

        // Foreign key to Candidate
        public Guid CandidateId { get; set; }
        public Candidat Candidat { get; set; }

        // Foreign key to JobOffer
        public Guid JobOfferId { get; set; }
        public JobOffer JobOffer { get; set; }
    }
}