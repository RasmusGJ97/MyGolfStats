namespace MyGolfStatsApi.DTOs
{
    public class RegisterRequestDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GolfId { get; set; }
        public string Email { get; set; }   
        public string Password { get; set; }
        public decimal Hcp { get; set; }
        public string Role { get; set; } = "User";
    }
}
