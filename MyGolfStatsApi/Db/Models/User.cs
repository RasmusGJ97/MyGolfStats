using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGolfStatsApi.Db.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }        
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }
        [Required]
        [Range(-8, 54)]
        public decimal Hcp { get; set; }
        [Required]
        [MaxLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string GolfId { get; set; }
        public string PasswordHash { get; set; }
        [Required]
        [MaxLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string Role { get; set; }
        [Required]
        public Settings Settings { get; set; }
        public List<Round> Rounds { get; set; }
        public Bag Bag { get; set; }
    }
}
