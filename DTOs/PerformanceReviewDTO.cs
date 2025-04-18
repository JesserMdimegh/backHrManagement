using System.ComponentModel.DataAnnotations;

namespace Back_HR.DTOs
{
    public class PerformanceReviewDTO
    {
        public Guid EmployeeId { get; set; }
        public int TasksCompleted { get; set; }
        public double OnTimeCompletionRate { get; set; } // Percentage (e.g., 95.5)
        public int ProcessImprovementIdeas { get; set; }
        public int Absences { get; set; }
        public int LateArrivals { get; set; }

        // Qualitative scores (1-5 scale)
        [Range(1, 5)]
        public int OutputQualityScore { get; set; }  // Based on peer/client reviews
        [Range(1, 5)]
        public int InitiativeScore { get; set; }     // Proactiveness
        [Range(1, 5)]
        public int CommunicationScore { get; set; }  // Clarity in emails/reports

        public string ManagerComment { get; set; }
        public int ClientSatisfactionScore { get; set; }
    }
}
