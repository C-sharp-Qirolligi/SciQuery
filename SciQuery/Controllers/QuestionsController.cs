using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.Interfaces;
using SciQuery.Service.QueryParams;
using SciQuery.Service.Services;
using System.Security.Claims;

namespace SciQuery.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("AllowLocalhost5173")]
public class QuestionsController(IQuestionService questionService, UserManager<User> userManager) : ControllerBase
{

    private readonly IQuestionService _questionService = questionService;
    private readonly UserManager<User> _userManager = userManager;

    [HttpGet("get-with-tags")]
    public async Task<ActionResult> GetQuestionsByTags([FromBody] QuestionQueryParameters queryParameters)
    {
        var result = await _questionService.GetQuestionsByTags(queryParameters);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuestions([FromQuery] QuestionQueryParameters queryParameters)
    {
        var questions = await _questionService.GetAllAsync(queryParameters);
        return Ok(questions);
    }


    [HttpPost]
    public async Task<IActionResult> CreateQuestion([FromBody] QuestionForCreateDto questionDto)
    {
        questionDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new EntityNotFoundException("User does not found!");


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionForCreateDto question)
        {
            // Foydalanuvchini topish
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new EntityNotFoundException("User does not found!");
            }

            question.UserId = user.Id;

            // ModelState ni tekshirish
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Savol yaratish jarayoni
                var createdQuestion = await _questionService.CreateAsync(question);

                // Yaratilgan savolni qaytarish
                return CreatedAtAction(nameof(GetQuestionById),
                       new { id = createdQuestion.Id },
                       createdQuestion);
            }
            catch (Exception ex)
            {
                // Har qanday boshqa xatolar uchun umumiy xatolik javobini qaytarish
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        var createdQuestion = await _questionService.CreateAsync(questionDto);
        return Created(nameof(GetQuestionById), new { createdQuestion });
    }


    [HttpPost("UploadImages")]
    public async Task<ActionResult> UploadFile(List<IFormFile> files)
    {
        var result = await _questionService.CreateImages(files);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionForUpdateDto questionDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _questionService.UpdateAsync(id, questionDto);

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        await _questionService.DeleteAsync(id);

        return NoContent();
    }
}