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

        public async Task<bool> CheckIfUserExists(string golfId, string email)
        {
            try
            {
                var getId = await _context.Users.AnyAsync(u => u.GolfId == golfId);
                var getEmail = await _context.Users.AnyAsync(u => u.Email == email);
                if (getId || getEmail)
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
                    Email = req.Email,
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

                var mappedUser = userMappingMethod(existingUser, userToUpdate);

                await _context.SaveChangesAsync();

                return mappedUser;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while updating a user", ex);
            }
        }

        public async Task<bool> UpdatePassword(PasswordChangeDTO passwordChange)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(passwordChange.NewPassword) || passwordChange.NewPassword.Length < 8)
                {
                    throw new Exception("Nytt lösenord måste vara minst 8 tecken långt.");
                }

                var existingUser = await GetUserWithId(passwordChange.UserId);

                if (existingUser == null)
                {
                    throw new Exception($"User with ID {passwordChange.UserId} not found.");
                }

                if (!PasswordHasher.Verify(passwordChange.OldPassword, existingUser.PasswordHash))
                {
                    throw new Exception("Felaktigt nuvarande lösenord.");
                }

                existingUser.PasswordHash = PasswordHasher.Hash(passwordChange.NewPassword);

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while updating a password", ex);
            }
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            try
            {
                var existingUser = await GetUserWithId(userId);

                if (existingUser == null)
                {
                    throw new Exception($"User with ID {userId} not found.");
                } 

                _context.Users.Remove(existingUser);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Message: {ex.Message}. Method: {MethodBase.GetCurrentMethod().Name ?? ""}.");
                throw new Exception("An error occurred while deleting a user", ex);
            }
        }

        public User userMappingMethod(User existingUser, UserUpdateDTO userToUpdate)
        {
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
            existingUser.Bag.Driver = userToUpdate.Bag.Driver;
            existingUser.Bag.TwoW = userToUpdate.Bag.TwoW;
            existingUser.Bag.ThreeW = userToUpdate.Bag.ThreeW;
            existingUser.Bag.FourW = userToUpdate.Bag.FourW;
            existingUser.Bag.FiveW = userToUpdate.Bag.FiveW;
            existingUser.Bag.SixW = userToUpdate.Bag.SixW;
            existingUser.Bag.SevenW = userToUpdate.Bag.SevenW;
            existingUser.Bag.TwoHy = userToUpdate.Bag.TwoHy;
            existingUser.Bag.ThreeHy = userToUpdate.Bag.ThreeHy;
            existingUser.Bag.FourHy = userToUpdate.Bag.FourHy;
            existingUser.Bag.FiveHy = userToUpdate.Bag.FiveHy;
            existingUser.Bag.OneI = userToUpdate.Bag.OneI;
            existingUser.Bag.TwoI = userToUpdate.Bag.TwoI;
            existingUser.Bag.ThreeI = userToUpdate.Bag.ThreeI;
            existingUser.Bag.FourI = userToUpdate.Bag.FourI;
            existingUser.Bag.FiveI = userToUpdate.Bag.FiveI;
            existingUser.Bag.SixI = userToUpdate.Bag.SixI;
            existingUser.Bag.SevenI = userToUpdate.Bag.SevenI;
            existingUser.Bag.EightI = userToUpdate.Bag.EightI;
            existingUser.Bag.NineI = userToUpdate.Bag.NineI;
            existingUser.Bag.PWedge = userToUpdate.Bag.PWedge;
            existingUser.Bag.AWedge = userToUpdate.Bag.AWedge;
            existingUser.Bag.GWedge = userToUpdate.Bag.GWedge;
            existingUser.Bag.SWedge = userToUpdate.Bag.SWedge;
            existingUser.Bag.LWedge = userToUpdate.Bag.LWedge;

            return existingUser;
        }
        public async Task<UserResponseDTO> MapToUserResponse(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                GolfId = user.GolfId,
                Role = user.Role,
                Hcp = user.Hcp,
                Settings = user.Settings,
                Rounds = user.Rounds,
                Bag = user.Bag
            };
        }
    }
}
