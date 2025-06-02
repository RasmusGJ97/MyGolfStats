using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGolfStatsApi.DTOs;
using MyGolfStatsApi.Services;

namespace MyGolfStatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : ControllerBase
    {
        private IRoundService _roundService;

        public RoundController(IRoundService roundService)
        {
            _roundService = roundService;
        }

        [HttpPut("updateRound")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdateRound([FromBody] RoundDTO round)
        {
            if (round == null)
            {
                return BadRequest("Invalid round data.");
            }

            var updatedRound = await _roundService.UpdateRound(round);

            if (updatedRound == null)
            {
                return NotFound($"Round with ID {round.Id} was not found.");
            }

            return Ok(new
            {
                message = "Round updated successfully.",
                round = updatedRound
            });
        }

        [HttpPost("addRound")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddRound([FromBody] AddRoundDTO round)
        {
            if (round == null)
            {
                return BadRequest("Invalid round data.");
            }

            var addedRound = await _roundService.AddRound(round);

            if (addedRound == null)
            {
                return NotFound($"Round could not be added");
            }

            return Ok(new
            {
                message = "Round added successfully.",
                round = addedRound
            });
        }

        [HttpDelete("deleteRound/{roundId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteRound(int roundId)
        {
            var removed = await _roundService.DeleteRound(roundId);

            if (removed == false)
            {
                return NotFound($"Round was not found.");
            }

            return Ok(new
            {
                message = "Round removed successfully.",
                roundRemoved = removed
            });
        }
    }
}
