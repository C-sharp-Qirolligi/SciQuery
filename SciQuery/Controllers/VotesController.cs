using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.Vote;
using SciQuery.Service.Interfaces;

namespace SciQuery.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VotesController(IVoteService voteService,UserManager<User> userManager) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;
    private readonly UserManager<User> _userManager = userManager;

    [HttpPost("upvote")]
    [Authorize]
    public async Task<IActionResult> UpVote([FromBody] VoteRequest vote)
    {
        var user = await _userManager.GetUserAsync(User)
              ?? throw new EntityNotFoundException("User does not found!");

        var result = await _voteService.UpVote(user.Id, vote.PostId, (PostType)vote.PostType);
       
        if (!result.Item1)
        {
            return BadRequest(result.Item2);
        }
        
        return Ok(result.Item2);
    }
    [HttpPost("downvote")]
    [Authorize]
    public async Task<IActionResult> DownVote([FromBody] VoteRequest vote)
    {
        var user = await _userManager.GetUserAsync(User)
              ?? throw new EntityNotFoundException("User does not found!");

        var result = await _voteService.DownVote(user.Id, vote.PostId, (PostType)vote.PostType);


        if (!result.Item1)
        {
            return BadRequest(result.Item2);
        }

        return Ok(result.Item2);
    }
}
