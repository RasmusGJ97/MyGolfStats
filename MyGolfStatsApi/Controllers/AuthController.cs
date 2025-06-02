using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.DTOs;
using MyGolfStatsApi.Services;

namespace MyGolfStatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IEmailService _emailService;

        public AuthController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPost("ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("Pong");
        }        
        
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO req)
        {
            if (_userService.CheckIfUserExists(req.GolfId, req.Email).Result)
            {
                return BadRequest("Golf-ID or email already exists.");
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "E-postadress krävs." });

            await _emailService.RequestPasswordReset(dto.Email);
            return Ok("Återställningslänk skickad om e-posten finns registrerad.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                await _emailService.ResetPassword(dto.Token, dto.NewPassword);
                return Ok("Lösenordet har återställts.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
