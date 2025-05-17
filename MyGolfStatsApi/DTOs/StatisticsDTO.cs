namespace MyGolfStatsApi.DTOs
{
    public class StatisticsDTO
    {
        public bool? FiR { get; set; }
        public bool? GiR { get; set; }
        public bool? Birdie { get; set; }
        public bool? Eagle { get; set; }
        public int? PenaltyStrokes { get; set; }
        public List<PenaltyCauseDTO>? PenaltyCause { get; set; }
        public bool? UpAndDown { get; set; }
        public bool? SandSave { get; set; }
        public int? Putts { get; set; }
        public int Strokes { get; set; }
        public int HoleId { get; set; }
        public int? LostBalls { get; set; }
        public int RoundId { get; set; }
    }
}
