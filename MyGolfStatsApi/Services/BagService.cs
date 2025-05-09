using Microsoft.EntityFrameworkCore;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;
using System.Reflection;

namespace MyGolfStatsApi.Services
{
    public class BagService : IBagService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _log;

        public BagService(AppDbContext context, ILogger<UserService> log)
        {
            _context = context;
            _log = log;
        }

        public async Task<User> GetBagAsync(Guid userId)
        {
            try
            {
                var userBag = await _context.Users.Include(u => u.Bag).FirstOrDefaultAsync(u => u.Id == userId);
                return userBag;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while finding a bag and user", ex);
            }
        }

        public async Task<bool> UpdateBagAsync(BagUpdateDTO bagUpdateDTO, Guid userId)
        {
            try
            {
                var userWithExistingBag = await GetBagAsync(userId);

                if (userWithExistingBag == null)
                {
                    throw new Exception($"User with ID {userId} not found.");
                }

                var properties = typeof(Bag).GetProperties();
                foreach (var prop in properties)
                {
                    var dtoProp = typeof(BagUpdateDTO).GetProperty(prop.Name);
                    if (dtoProp != null)
                    {
                        var value = dtoProp.GetValue(bagUpdateDTO);
                        prop.SetValue(userWithExistingBag.Bag, value);
                    }
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while updating a bag", ex);
            }
        }
    }
}
