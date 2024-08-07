using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Service.DTOs.Vote;
using SciQuery.Service.Interfaces;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController(IVoteService voteService) : ControllerBase
    {
        private readonly IVoteService _voteService = voteService;

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetVoteByUserId(string id)
        {
            var vote = await _voteService.GetVoteByUserIdAsync(id);
            if (vote == null)
            {
                return NotFound();
            }
            return Ok(vote);
        }

        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetAllVotesByQuestionId(int questionId)
        {
            var votes = await _voteService.GetAllVotesByQuestionIdAsync(questionId);
            return Ok(votes);
        }

        [HttpGet("answer/{answerId}")]
        public async Task<IActionResult> GetAllVotesByAnswerId(int answerId)
        {
            var votes = await _voteService.GetAllVotesByAnswerIdAsync(answerId);
            return Ok(votes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVote([FromBody] VoteForCreateDto voteCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdVote = await _voteService.CreateVoteAsync(voteCreateDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVote(int id)
        {
            var result = await _voteService.DeleteVoteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
