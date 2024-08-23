using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.Interfaces;
using SciQuery.Service.QueryParams;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(ICommentService commentService, UserManager<User> userManager) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCommentsByAnswerId(CommentQueryParameters queryParameters)
        {
            var comments = await _commentService.GetAllComments(queryParameters);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentForCreateDto commentCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Foydalanuvchini topish
            var user = await _userManager.GetUserAsync(User)
                ?? throw new EntityNotFoundException("User does not found!");

            commentCreateDto.UserId = user.Id;

            var createdComment = await _commentService.CreateCommentAsync(commentCreateDto);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentForUpdateDto commentUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Foydalanuvchini topish
            var user = await _userManager.GetUserAsync(User)
                ?? throw new EntityNotFoundException("User does not found!");

            commentUpdateDto.UserId = user.Id;

            var updatedComment = await _commentService.UpdateCommentAsync(id, commentUpdateDto);
           
            return Ok(updatedComment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var result = await _commentService.DeleteCommentAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
