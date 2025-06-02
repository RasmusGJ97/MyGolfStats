using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;

namespace MyGolfStatsApi.Services
{
    public interface IUserService
    {
        Task<bool> CheckIfUserExists(string golfId, string email);
        Task<User> RegisterUser(RegisterRequestDTO req);
        Task<User> GetUserWithGolfId(string golfId);
        Task<User> GetUserWithId(Guid id);
        string GenerateJwtToken(User user);
        Task<User> UpdateUser(UserUpdateDTO user);
        Task<bool> UpdatePassword(PasswordChangeDTO passwordChange);    
        Task<bool> DeleteUser(Guid userId);
        Task<UserResponseDTO> MapToUserResponse(User user);
    }
}
