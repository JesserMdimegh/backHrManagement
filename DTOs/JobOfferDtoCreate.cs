using Back_HR.Models;

namespace Back_HR.DTOs
{
    public class JobOfferDtoCreate
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Experience { get; set; }
        public Double Salary { get; set; }
        public String Location { get; set; }
        public List<Competence> Competences { get; set; } = new List<Competence>();
    }
}
