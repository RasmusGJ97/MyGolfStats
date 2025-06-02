namespace MyGolfStatsApi.DTOs
{
    public class AddCourseDTO
    {
        public string ClubName { get; set; }
        public decimal? CourseRating { get; set; }
        public List<string> Tees { get; set; }
        public List<HoleDTO> Holes { get; set; }
    }
}
