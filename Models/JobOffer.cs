using System.ComponentModel.DataAnnotations;
using Back_HR.Models.enums;

namespace Back_HR.Models
{
    public class JobOffer
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public int Experience { get; set; }
        public Double Salary { get; set; }
        public String Location { get; set; }
        public OffreStatus Status { get; set; } = OffreStatus.OPEN;

        // 1-to-1 with RH (HR manages this offer)
        public Guid RHId { get; set; }
        public RH RHResponsable { get; set; }

        // Many-to-many with Skill
        public List<Competence> Competences { get; set; } = new List<Competence>();

        // One-to-many with Application
        public List<Application> Applications { get; set; } = new List<Application>();
    }
}