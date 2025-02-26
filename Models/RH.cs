using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class RH : User
    {
        public List<JobOffer> Offres { get; set; } = new List<JobOffer>(); // 1-to-many with OffresEmploi
        public List<Candidat> Candidats { get; set; } = new List<Candidat>(); // 1-to-many with Candidat
    }
}