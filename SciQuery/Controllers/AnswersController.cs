using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Services;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController(IAnswerService answerService) : ControllerBase
    {
        private readonly IAnswerService _answerService = answerService;

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
        public async Task<IActionResult> CreateAnswer([FromBody] AnswerForCreateDto answerCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAnswer = await _answerService.CreateAsync(answerCreateDto);
            return CreatedAtAction(nameof(GetAnswerById), new { id = createdAnswer.Id }, createdAnswer);
        }

        [HttpPost("UploadImages")]
        public async Task<ActionResult> UploadFile(List<IFormFile> files)
        {
            var result = await _answerService.CreateImages(files);
            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] AnswerForUpdateDto answerUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
