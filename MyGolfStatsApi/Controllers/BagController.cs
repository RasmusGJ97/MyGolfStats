
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGolfStatsApi.DTOs;
using MyGolfStatsApi.Services;

namespace MyGolfStatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BagController : ControllerBase
    {
        private IBagService _bagService;

        public BagController(IBagService bagService)
        {
            _bagService = bagService;
        }

        [HttpPut("updateBag")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdateBag([FromBody] BagUpdateDTO bagToUpdate)
        {
            if (bagToUpdate == null || bagToUpdate.UserId == Guid.Empty)
            {
                return BadRequest("Invalid user data.");
            }

            var updatedBag = await _bagService.UpdateBagAsync(bagToUpdate, bagToUpdate.UserId);

            if (!updatedBag)
            {
                return NotFound($"Bag for user with ID {bagToUpdate.UserId} was not found.");
            }

            return Ok(new
            {
                message = "Bag updated successfully.",
                bag = updatedBag
            });
        }
    }
}
