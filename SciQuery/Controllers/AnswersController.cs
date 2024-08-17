using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Services;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController(IAnswerService answerService,UserManager<User>userManager) : ControllerBase
    {
        private readonly IAnswerService _answerService = answerService;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerById(int id)
        {
            var answer = await _answerService.GetByIdAsync(id);
            if (answer == null)
            {
                return NotFound();
            }
            return Ok(answer);
        }

        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetAllAnswersByQuestionId(int questionId)
        {
            var answers = await _answerService.GetAllAnswersByQuestionIdAsync(questionId);
            return Ok(answers);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAnswer([FromBody] AnswerForCreateDto answerCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Foydalanuvchini topish
            var user = await _userManager.GetUserAsync(User)
                ?? throw new EntityNotFoundException("User does not found!");

            answerCreateDto.UserId = user.Id;

            var createdAnswer = await _answerService.CreateAsync(answerCreateDto);
            return CreatedAtAction(nameof(GetAnswerById), new { id = createdAnswer.Id }, createdAnswer);
        }

        [HttpPost("UploadImages")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            var result = await _answerService.CreateImages(file);
            return Ok(result);
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] AnswerForUpdateDto answerUpdateDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Foydalanuvchini topish
            var user = await _userManager.GetUserAsync(User)
                ?? throw new EntityNotFoundException("User does not found!");

            answerUpdateDto.UserId = user.Id;

            await _answerService.UpdateAsync(id, answerUpdateDto);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            await _answerService.DeleteAsync(id);

            return NoContent();
        }
    }
}
