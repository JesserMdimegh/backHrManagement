namespace Back_HR.Models
{
    public class Candidat : User
    {
        public string Cv { get; set; }
        public List<Competence> Competences { get; set; } = new List<Competence>(); // many-to-many with Competence
        public List<Application> Applications { get; set; } = new List<Application>(); // One-to-many with Application
    }
}

