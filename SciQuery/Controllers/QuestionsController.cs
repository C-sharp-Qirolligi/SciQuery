﻿//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using SciQuery.Domain.Entities;
//using SciQuery.Domain.Exceptions;
//using SciQuery.Domain.UserModels;
//using SciQuery.Service.DTOs.Question;
//using SciQuery.Service.DTOs.Tag;
//using SciQuery.Service.Interfaces;
//using SciQuery.Service.Pagination.PaginatedList;
//using SciQuery.Service.QueryParams;
//using System.Security.Claims;

//namespace SciQuery.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [EnableCors("AllowLocalhost5173")]
//    public class QuestionsController(IQuestionService questionService) : ControllerBase
//    {
//        private readonly IQuestionService _questionService = questionService;


//        [HttpGet("get-with-tags")]
//        public async Task<ActionResult> GetQuestionsByTags([FromBody] QuestionQueryParameters queryParams)
//        {
//            var result = await _questionService.GetQuestionsByTags(queryParams);
//            return Ok(result);
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetAllQuestions([FromQuery] QuestionQueryParameters queryParameters)
//        {
//            var questions = await _questionService.GetAllAsync(queryParameters);
//            return Ok(questions);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetQuestionById(int id)
//        {
//            var question = await _questionService.GetByIdAsync(id);
//            if (question == null)
//            {
//                return NotFound();
//            }
//            return Ok(question);
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateQuestion([FromBody] QuestionForCreateDto question)
//        {
//            // UserId ni aniqlash
//            question.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
//                ?? throw new EntityNotFoundException("User does not found!");

//            // ModelState ni tekshirish
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                // Savol yaratish jarayoni
//                var createdQuestion = await _questionService.CreateAsync(question);

//                // Yaratilgan savolni qaytarish
//                return CreatedAtAction(nameof(GetQuestionById),
//                       new { id = createdQuestion.Id }, 
//                       createdQuestion);
//            }
//            catch (Exception ex)
//            {
//                // Har qanday boshqa xatolar uchun umumiy xatolik javobini qaytarish
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }


//        //[HttpPost]
//        //public async Task<IActionResult> CreateQuestion([FromBody] QuestionForCreateDto questionDto)
//        //{
//        //    questionDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
//        //        ?? throw new EntityNotFoundException("User does not found!");

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return BadRequest(ModelState);
//        //    }

//        //    var createdQuestion = await _questionService.CreateAsync(questionDto);
//        //    return Created(nameof(GetQuestionById),new { createdQuestion });
//        //}

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionForUpdateDto questionDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            await _questionService.UpdateAsync(id, questionDto);

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteQuestion(int id)
//        {
//            await _questionService.DeleteAsync(id);

//            return NoContent();
//        }
//    }
//}

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;
using System.Security.Claims;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost5173")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly UserManager<User> _userManager;

        public QuestionsController(IQuestionService questionService, UserManager<User> userManager)
        {
            _questionService = questionService;
            _userManager = userManager;
        }

        [HttpGet("get-with-tags")]
        public async Task<ActionResult> GetQuestionsByTags([FromBody] QuestionQueryParameters queryParams)
        {
            var result = await _questionService.GetQuestionsByTags(queryParams);
            return Ok(result);
        }

        [HttpGet]
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
}
