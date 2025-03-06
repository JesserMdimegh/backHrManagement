namespace Back_HR.Models
{
    public class RegisterModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public string UserType { get; set; } // "RH", "Employe", "Candidat"
        public string? Poste { get; set; } // For Employe
        public string? Cv { get; set; }
    }
}
