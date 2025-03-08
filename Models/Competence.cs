using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Back_HR.Models
{
    public class Competence
    {
        [Key]
        public Guid Id { get; set; }
        public string Titre { get; set; }


        // Many-to-many with Candidat
        [JsonIgnore]
        public List<Candidat> Candidats { get; set; } = new List<Candidat>();

        // Many-to-many with OffresEmploi
        [JsonIgnore]
        public List<JobOffer> JobOffres { get; set; } = new List<JobOffer>();
    }
}