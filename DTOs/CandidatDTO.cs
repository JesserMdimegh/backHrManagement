using Back_HR.Models;

namespace Back_HR.DTOs
{
    public class CandidatDTO
    {
        public Guid Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public List<Competence> Competences { get; set; }
        public string Cv { get; set; }
    }
}
