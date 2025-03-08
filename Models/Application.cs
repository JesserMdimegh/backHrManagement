using System.ComponentModel.DataAnnotations;
using Back_HR.Models.enums;

namespace Back_HR.Models
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ApplicationDate { get; set; } = DateTime.Now; // When the application was submitted
        public ApplicationStatus Status { get; set; } = ApplicationStatus.PENDING;

        // Foreign key to Candidate
        public Guid CandidateId { get; set; }
        public Candidat Candidat { get; set; }

        // Foreign key to JobOffer
        public Guid JobOfferId { get; set; }
        public JobOffer JobOffer { get; set; }
    }
}