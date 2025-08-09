namespace MyGolfStatsApi.Db.Models
{
    public class ApiLogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public long ResponseTimeMs { get; set; }
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
    }
}
