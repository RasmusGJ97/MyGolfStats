using MyGolfStatsApi.Db.Models;

namespace MyGolfStatsApi.DTOs
{
    public class UserUpdateDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Hcp { get; set; }
        public SettingsDTO Settings { get; set; }
        public BagUpdateDTO Bag { get; set; }   
    }
}
