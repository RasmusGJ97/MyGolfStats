using MyGolfStatsApi.Db.Models;

namespace MyGolfStatsApi.DTOs
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string GolfId { get; set; }
        public string Role { get; set; }
        public decimal Hcp { get; set; }
        public Settings Settings { get; set; }
        public List<Round> Rounds { get; set; }
        public Bag Bag { get; set; }
    }
}
