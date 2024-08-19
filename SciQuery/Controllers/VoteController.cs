using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.Interfaces;

namespace SciQuery.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoteController(IVoteService voteService,UserManager<User> userManager) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;
    private readonly UserManager<User> _userManager = userManager;

    [Authorize]
    public async Task<IActionResult> UpVote(int postId,PostType postType)
    {
        var user = await _userManager.GetUserAsync(User)
              ?? throw new EntityNotFoundException("User does not found!");

        await _voteService.UpVote(user.Id, postId, postType);
        return Ok();
    }
    [Authorize]
    public async Task<IActionResult> DownVote(int postId,PostType postType)
    {
        var user = await _userManager.GetUserAsync(User)
              ?? throw new EntityNotFoundException("User does not found!");

        await _voteService.UpVote(user.Id, postId, postType);

        return Ok();
    }
}
