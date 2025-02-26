using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class Survey
    {
        [Key]
        public Guid Id { get; set; }
        public List<string> Questions { get; set; } = new List<string>();

        // One-to-many with SurveyResponse
        public List<SurveyResponse> Responses { get; set; } = new List<SurveyResponse>();

        // many-to-many with Employe
        public List<Employe> Employes { get; set; } = new List<Employe>();
    }
}