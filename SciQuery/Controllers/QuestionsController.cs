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

    [HttpGet("get-by-tags/{id}")]
    public async Task<ActionResult> GetQuestionsByTags(int id)
    {
        var result = await _questionService.GetQuestionsByTags(id);
        return Ok(result);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllQuestions([FromQuery] QuestionQueryParameters queryParameters)
    {
        var questions = await _questionService.GetAllAsync(queryParameters);
        return Ok(questions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestionById(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question == null)
        {
            return NotFound();
        }
        return Ok(question);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] QuestionForCreateDto question)
    {
        // Foydalanuvchini topish
            var user = await _userManager.GetUserAsync(User)
            ?? throw new EntityNotFoundException("User does not found!");
        
        question.UserId = user.Id;

        // ModelState ni tekshirish
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Savol yaratish jarayoni
        var createdQuestion = await _questionService.CreateAsync(question);

        // Yaratilgan savolni qaytarish
        return CreatedAtAction(nameof(GetQuestionById),
               new { id = createdQuestion.Id }, createdQuestion);
    }

    [HttpPost("upload-image")]
    [Authorize]
    public async Task<ActionResult> UploadFile( IFormFile file)
    {
        var result = await _questionService.CreateImages(file);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var result = await _questionService.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}