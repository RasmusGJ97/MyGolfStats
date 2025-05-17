using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MyGolfStatsApi.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _log;

        public UserService(IConfiguration config, AppDbContext context, ILogger<UserService> log)
        {
            _config = config;
            _context = context;
            _log = log;
        }

        public async Task<bool> CheckIfUserExists(string golfId)
        {
            try
            {
                var getId = await _context.Users.AnyAsync(u => u.GolfId == golfId);
                if (getId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while trying to find a user.", ex);
            }
        }

        public async Task<User> RegisterUser(RegisterRequestDTO req)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = req.FirstName,
                    LastName = req.LastName,
                    GolfId = req.GolfId,
                    Role = req.Role,
                    Hcp = req.Hcp,
                    PasswordHash = PasswordHasher.Hash(req.Password),
                    Bag = new Bag(),
                    Settings = new Settings()
                    {
                        StatFiR = true,
                        StatGiR = true,
                        StatBirdie = true,
                        StatEagle = true,
                        StatPenaltyStrokes = true,
                        StatPenaltyCause = true,
                        StatUpAndDown = true,
                        StatSandSave = true,
                        StatPutts = true,
                        StatLostBalls = true
                    },
                    Rounds = new List<Round>()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while registrating a user", ex);
            }
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.GolfId),
            new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserWithGolfId(string golfId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.GolfId == golfId);

                return user;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while finding a user", ex);
            }
        }

        public async Task<User> GetUserWithId(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Settings)
                    .Include(u => u.Bag)
                    .Include(u => u.Rounds)
                        .ThenInclude(r => r.Statistics)
                            .ThenInclude(s => s.Hole)
                    .Include(u => u.Rounds)
                        .ThenInclude(r => r.Statistics)
                            .ThenInclude(s => s.PenaltyCause)
                    .FirstOrDefaultAsync(u => u.Id == id);

                return user;

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while finding a user", ex);
            }
        }

        public async Task<User> UpdateUser(UserUpdateDTO userToUpdate)
        {
            try
            {
                var existingUser = await GetUserWithId(userToUpdate.Id);

                if (existingUser == null)
                {
                    throw new Exception($"User with ID {userToUpdate.Id} not found.");
                }

                existingUser.FirstName = userToUpdate.FirstName;
                existingUser.LastName = userToUpdate.LastName;
                existingUser.Hcp = userToUpdate.Hcp;
                existingUser.Settings.StatFiR = userToUpdate.Settings.StatFiR;
                existingUser.Settings.StatGiR = userToUpdate.Settings.StatGiR;
                existingUser.Settings.StatBirdie = userToUpdate.Settings.StatBirdie;
                existingUser.Settings.StatEagle = userToUpdate.Settings.StatEagle;
                existingUser.Settings.StatPenaltyStrokes = userToUpdate.Settings.StatPenaltyStrokes;
                existingUser.Settings.StatPenaltyCause = userToUpdate.Settings.StatPenaltyCause;
                existingUser.Settings.StatUpAndDown = userToUpdate.Settings.StatUpAndDown;
                existingUser.Settings.StatSandSave = userToUpdate.Settings.StatSandSave;
                existingUser.Settings.StatPutts = userToUpdate.Settings.StatPutts;
                existingUser.Settings.StatLostBalls = userToUpdate.Settings.StatLostBalls;

                await _context.SaveChangesAsync();

                return existingUser;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while updating a user", ex);
            }
        }
    }
}
