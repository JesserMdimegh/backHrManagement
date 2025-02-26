using System.ComponentModel.DataAnnotations;

namespace Back_HR.Models
{
    public class Employe : User
    {
        public string Poste { get; set; }
        public List<Survey> SurveysResponded { get; set; } = new List<Survey>(); // 1-to-many with Sondage

        // One-to-many with SurveyResponse
        public List<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
    }
}