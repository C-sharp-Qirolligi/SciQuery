using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Tag;
using SciQuery.Service.Interfaces;
using SciQuery.Service.QueryParams;

namespace SciQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController(ITagService tagService) : ControllerBase
    {
        private readonly ITagService _tagService = tagService;

        [HttpGet]
        public async Task<IActionResult> GetAllTags([FromQuery] TagQueryParameters queryParameters)
        {
            var tags = await _tagService.GetAllTagsAsync(queryParameters);
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagForCreateDto tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTag = await _tagService.CreateTagAsync(tag);
            return CreatedAtAction(nameof(GetTagById), new { id = createdTag.Id }, createdTag);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedTag = await _tagService.UpdateTagAsync(id, tag);
            if (updatedTag == null)
            {
                return NotFound();
            }

            return Ok(updatedTag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var result = await _tagService.DeleteTagAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
