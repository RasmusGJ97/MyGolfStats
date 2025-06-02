namespace MyGolfStatsApi.DTOs
{
    public class AddRoundDTO
    {
        public int NettoScore { get; set; }
        public DateOnly Date { get; set; }
        public string Tee { get; set; }
        public Guid UserId { get; set; }
        public List<StatisticsDTO> Statistics { get; set; }
    }
}
