namespace MyGolfStatsApi.DTOs
{
    public class PenaltyCauseDTO
    {
        public string Cause { get; set; } = "";
        public string Club { get; set; } = "";
        public int PenaltyStrokes { get; set; }
    }
}
