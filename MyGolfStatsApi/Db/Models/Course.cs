using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGolfStatsApi.Db.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string ClubName { get; set; }
        [Range(0, 155)]
        public decimal? CourseRating { get; set; }
        [Required]
        [Range(27, 80)]
        public int Par { get; set; }
        [Required]
        public List<string> Tees { get; set; }
        [Required]
        public List<Hole> Holes { get; set; }
    }
}
