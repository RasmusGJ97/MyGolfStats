using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyGolfStatsApi.Db.Models
{
    public class Hole
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 18)]
        public int HoleNumber { get; set; }
        [Required]
        [Range(3, 6)]
        public int Par { get; set; }
        [Required]
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        [JsonIgnore]
        public Course Course { get; set; }
    }
}
