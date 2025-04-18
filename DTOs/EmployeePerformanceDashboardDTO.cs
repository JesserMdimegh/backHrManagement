namespace Back_HR.DTOs
{
    public class EmployeePerformanceDashboardDTO
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Poste { get; set; }
        public int AvgTasksCompleted { get; set; }
        public double AvgOnTimeCompletionRate { get; set; }
        public int TotalAbsences { get; set; }
        public double AvgOutputQualityScore { get; set; }
        public double OverallPerformanceScore { get; set; }
        public string PerformanceTrend { get; set; } // "Improving", "Declining", "Stable"
    }
}
