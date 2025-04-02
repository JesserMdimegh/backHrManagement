namespace Back_HR.DTOs
{
    public class ApplicationDtoPost
    {
        public Guid CandidatId { get; set; }
        public Guid JobOfferId { get; set; }
        public IFormFile? Cv { get; set; }

        public override string ToString()
        {
            return $"CandidatId: {CandidatId}, JobOfferId: {JobOfferId}, " +
                   $"CV: {(Cv != null ? $"Name: {Cv.FileName}, Size: {Cv.Length}" : "null")}";
        }
    }

}