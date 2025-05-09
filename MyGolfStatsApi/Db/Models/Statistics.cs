using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyGolfStatsApi.Db.Models
{
    public class Statistics
    {
        [Key]
        public int Id { get; set; }
        public bool? FiR { get; set; }
        public bool? GiR { get; set; }
        public bool? Birdie { get; set; }
        public bool? Eagle { get; set; }
        [Range(0, 10)]
        public int? PenaltyStrokes { get; set; }
        public List<PenaltyCause>? PenaltyCause { get; set; }
        public bool? UpAndDown { get; set; }
        public bool? SandSave { get; set; }
        [Range(0, 10)]
        public int? Putts { get; set; }
        [Required]
        [Range(1, 30)]
        public int Strokes { get; set; }
        [Required]
        public int HoleId { get; set; }
        [ForeignKey(nameof(HoleId))]
        public Hole Hole { get; set; }
        [Range(0, 10)]
        public int? LostBalls { get; set; }
        [Required]
        public int RoundId { get; set; }
        [ForeignKey(nameof(RoundId))]
        [JsonIgnore]
        public Round Round { get; set; }

    }
}
