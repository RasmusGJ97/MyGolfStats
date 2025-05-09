using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;

namespace MyGolfStatsApi.Services
{
    public interface IBagService
    {
        Task<User> GetBagAsync(Guid userId);
        Task<bool> UpdateBagAsync(BagUpdateDTO bagUpdateDTO, Guid userId);
    }
}
