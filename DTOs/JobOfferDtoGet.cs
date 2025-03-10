using Back_HR.Models;
using Back_HR.Models.enums;

namespace Back_HR.DTOs
{
    public class JobOfferDtoGet
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Experience { get; set; }
        public DateTime PublishDate { get; set; }
        public Double Salary { get; set; }
        public String Location { get; set; }
        public OffreStatus Status { get; set; }
        public List<Competence> Competences { get; set; } = new List<Competence>();
    }
}
