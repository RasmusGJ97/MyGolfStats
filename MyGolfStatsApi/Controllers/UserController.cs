using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGolfStatsApi.DTOs;
using MyGolfStatsApi.Services;

namespace MyGolfStatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("updateUser")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userToUpdate)
        {
            if (userToUpdate == null || userToUpdate.Id == Guid.Empty)
            {
                return BadRequest("Invalid user data.");
            }

            var updatedUser = await _userService.UpdateUser(userToUpdate);

            if (updatedUser == null)
            {
                return NotFound($"User with ID {userToUpdate.Id} was not found.");
            }

            return Ok(new
            {
                message = "User updated successfully.",
                user = updatedUser
            });
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user data.");
            }

            var userInfo = await _userService.GetUserWithId(userId);

            if (userInfo == null)
            {
                return NotFound($"User with ID {userId} was not found.");
            }

            return Ok(new
            {
                message = "User found successfully.",
                user = userInfo
            });
        }
    }
}
