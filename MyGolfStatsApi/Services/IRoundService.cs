using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;

namespace MyGolfStatsApi.Services
{
    public interface IRoundService
    {
        Task<Round> AddRound(AddRoundDTO round);
        Task<Round> UpdateRound(RoundDTO round);
        Task<bool> DeleteRound(int roundId);
    }
}
