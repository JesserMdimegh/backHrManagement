using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class Competence
    {
        [Key]
        public Guid Id { get; set; }
        public string Titre { get; set; }


        // Many-to-many with Candidat
        public List<Candidat> Candidats { get; set; } = new List<Candidat>();

        // Many-to-many with OffresEmploi
        public List<JobOffer> JobOffres { get; set; } = new List<JobOffer>();
    }
}