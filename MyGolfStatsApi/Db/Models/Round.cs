using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyGolfStatsApi.Db.Models
{
    public class Round
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(0, 200)]
        public int BruttoScore { get; set; }
        [Required]
        [Range(0, 200)]
        public int NettoScore { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Tee { get; set; }
        [Required]
        [Range(1, 18)]
        public int HolesPlayed { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User User { get; set; }
        public List<Statistics> Statistics { get; set; }
    }
}
