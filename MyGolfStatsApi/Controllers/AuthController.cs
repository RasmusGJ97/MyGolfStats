using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using MyGolfStatsApi.DTOs;
using MyGolfStatsApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyGolfStatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO req)
        {
            if (_userService.CheckIfUserExists(req.GolfId).Result)
            {
                return BadRequest("Golf-ID already exists.");
            }
            else
            {
                await _userService.RegisterUser(req);
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO req)
        {
            var user = await _userService.GetUserWithGolfId(req.GolfId);
            if (user == null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _userService.GenerateJwtToken(user);

            return Ok(new { token, user.Id });
        }
    }
}
