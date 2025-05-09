using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyGolfStatsApi.Db.Models
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User User { get; set; }
        [Required]
        public bool StatFiR { get; set; }
        [Required]
        public bool StatGiR { get; set; }
        [Required]
        public bool StatBirdie { get; set; }
        [Required]
        public bool StatEagle { get; set; } 
        [Required]
        public bool StatPenaltyStrokes { get; set; }
        [Required]
        public bool StatPenaltyCause { get; set; }
        [Required]
        public bool StatUpAndDown { get; set; }
        [Required]
        public bool StatSandSave { get; set; }
        [Required]
        public bool StatPutts { get; set; }
        [Required]
        public bool StatLostBalls { get; set; }
    }
}
