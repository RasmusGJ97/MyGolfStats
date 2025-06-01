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
        [Authorize(Roles = "User,Admin")]
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

            var response = _userService.MapToUserResponse(updatedUser);

            return Ok(new
            {
                message = "User updated successfully.",
                user = response.Result
            });
        }          
        
        [HttpPut("updatePassword")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordChangeDTO passwordChange)
        {
            if (passwordChange == null || passwordChange.UserId == Guid.Empty)
            {
                return BadRequest("Invalid password data.");
            }

            var updatedPassword = await _userService.UpdatePassword(passwordChange);

            if (updatedPassword == null)
            {
                return NotFound($"User with ID {passwordChange.UserId} was not found.");
            }

            return Ok(new
            {
                message = "Password updated successfully.",
                user = updatedPassword
            });
        }        
        
        [HttpDelete("deleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromBody] Guid userId)
        {
            if (userId == null || userId == Guid.Empty)
            {
                return BadRequest("Invalid user data.");
            }

            var deletedUser = await _userService.DeleteUser(userId);

            if (deletedUser == false)
            {
                return NotFound($"User with ID {userId} was not found.");
            }

            return Ok(new
            {
                message = "User deleted successfully.",
                user = deletedUser
            });
        }

        [HttpGet("user")]
        [Authorize(Roles = "User,Admin")]
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

            var response = _userService.MapToUserResponse(userInfo);

            return Ok(new
            {
                message = "User found successfully.",
                user = response.Result
            });
        }
    }
}
