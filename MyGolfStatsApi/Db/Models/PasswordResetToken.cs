using System.ComponentModel.DataAnnotations;

namespace MyGolfStatsApi.Db.Models
{
    public class PasswordResetToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;
    }
}
