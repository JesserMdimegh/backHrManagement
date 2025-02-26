using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class SurveyResponse
    {
        [Key]
        public Guid Id { get; set; }
        public string Response { get; set; } // The employee's response (string)

        // Foreign key to Survey
        public Guid SurveyId { get; set; }
        public Survey Survey { get; set; }

        // Foreign key to Employee
        public Guid EmployeeId { get; set; }
        public Employe Employee { get; set; }
    }
}