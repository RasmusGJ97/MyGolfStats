using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyGolfStatsApi.Db.Models
{
    public class PenaltyCause
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Cause { get; set; }
        [Required]
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string Club { get; set; }
        [Required]
        public int PenaltyStrokes { get; set; }
        [JsonIgnore]
        public List<Statistics> Statistics { get; set; }
    }
}
