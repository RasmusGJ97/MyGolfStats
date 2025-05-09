
using Microsoft.EntityFrameworkCore;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;
using System.Reflection;

namespace MyGolfStatsApi.Services
{
    public class RoundService : IRoundService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoundService> _log;

        public RoundService(AppDbContext context, ILogger<RoundService> log)
        {
            _context = context;
            _log = log;
        }
        public async Task<Round> AddRound(AddRoundDTO round)
        {
            try
            {
                var newRound = new Round
                {
                    NettoScore = round.NettoScore,
                    Date = round.Date,  
                    Tee = round.Tee,
                    UserId = round.UserId,
                    Statistics = new List<Statistics>(),
                };
                foreach (var stat in round.Statistics)
                {
                    var newStat = new Statistics
                    {
                        FiR = stat.FiR,
                        GiR = stat.GiR,
                        Birdie = stat.Birdie,
                        Eagle = stat.Eagle,
                        PenaltyStrokes = stat.PenaltyStrokes,
                        PenaltyCause = new List<PenaltyCause>(),
                        UpAndDown = stat.UpAndDown,
                        SandSave = stat.SandSave,
                        Putts = stat.Putts,
                        Strokes = stat.Strokes,
                        HoleId = stat.HoleId,
                        LostBalls = stat.LostBalls,
                        Round = newRound,
                    };
                    foreach (var pc in stat.PenaltyCause)
                    {
                        var existingCause = await _context.PenaltyCauses
                            .FirstOrDefaultAsync(p => p.Cause == pc.Cause &&
                            p.Club == pc.Club &&
                            p.PenaltyStrokes == pc.PenaltyStrokes);

                        if (existingCause != null)
                        {
                            newStat.PenaltyCause.Add(existingCause);
                        }
                        else
                        {
                            var newCause = new PenaltyCause
                            {
                                Cause = pc.Cause,
                                Club = pc.Club,
                                PenaltyStrokes = pc.PenaltyStrokes
                            };
                            newStat.PenaltyCause.Add(newCause);
                        }
                    }

                    newRound.BruttoScore += newStat.Strokes;
                    newRound.HolesPlayed ++;
                    newRound.Statistics.Add(newStat);
                }

                _context.Rounds.Add(newRound);
                await _context.SaveChangesAsync();


                return newRound;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod()?.Name ?? ""}.");
                throw new Exception("An error occurred while trying to add a round.", ex);
            }
        }

        public async Task<bool> DeleteRound(int roundId)
        {
            try
            {
                var existingRound = await _context.Rounds.FirstOrDefaultAsync(c => c.Id == roundId);
                if (existingRound == null)
                {
                    throw new Exception($"Round with ID {roundId} not found.");
                }

                _context.Rounds.Remove(existingRound);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod()?.Name ?? ""}.");
                throw new Exception("An error occurred while trying to delete a round.", ex);
            }
        }

        public async Task<Round> UpdateRound(RoundDTO round)
        {
            try
            {
                var existingRound = await _context.Rounds
                    .Include(c => c.Statistics)
                    .ThenInclude(s => s.PenaltyCause)
                    .FirstOrDefaultAsync(c => c.Id == round.Id);

                if (existingRound == null)
                {
                    throw new Exception($"Round with ID {round.Id} not found.");
                }

                existingRound.NettoScore = round.NettoScore;
                existingRound.Date = round.Date;
                existingRound.Tee = round.Tee;
                existingRound.UserId = round.UserId;

                _context.Statistics.RemoveRange(existingRound.Statistics);

                existingRound.Statistics = new List<Statistics>();
                existingRound.BruttoScore = 0;
                existingRound.HolesPlayed = 0;

                foreach (var stat in round.Statistics)
                {
                    var newStat = new Statistics
                    {
                        FiR = stat.FiR,
                        GiR = stat.GiR,
                        Birdie = stat.Birdie,
                        Eagle = stat.Eagle,
                        PenaltyStrokes = stat.PenaltyStrokes,
                        PenaltyCause = new List<PenaltyCause>(),
                        UpAndDown = stat.UpAndDown,
                        SandSave = stat.SandSave,
                        Putts = stat.Putts,
                        Strokes = stat.Strokes,
                        HoleId = stat.HoleId,
                        LostBalls = stat.LostBalls,
                        Round = existingRound,
                    };
                    foreach (var pc in stat.PenaltyCause)
                    {
                        var existingCause = await _context.PenaltyCauses
                            .FirstOrDefaultAsync(p => p.Cause == pc.Cause &&
                            p.Club == pc.Club &&
                            p.PenaltyStrokes == pc.PenaltyStrokes);

                        if (existingCause != null)
                        {
                            newStat.PenaltyCause.Add(existingCause);
                        }
                        else
                        {
                            var newCause = new PenaltyCause
                            {
                                Cause = pc.Cause,
                                Club = pc.Club,
                                PenaltyStrokes = pc.PenaltyStrokes
                            };
                            newStat.PenaltyCause.Add(newCause);
                        }
                    }

                    existingRound.BruttoScore += newStat.Strokes;
                    existingRound.HolesPlayed++;
                    existingRound.Statistics.Add(newStat);
                }

                await _context.SaveChangesAsync();
                return existingRound;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod()?.Name ?? ""}.");
                throw new Exception("An error occurred while trying to update a round.", ex);
            }
        }
    }
}
