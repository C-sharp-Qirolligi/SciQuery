using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.DTOs.Tag;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;
using SciQuery.Service.QueryParams;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SciQuery.Service.Services;

public class TagService(SciQueryDbContext context , IMapper mapper) : ITagService
{

    private readonly SciQueryDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedList<TagDto>> GetAllTagsAsync(TagQueryParameters queryParams)
    {
        var tags = _context.Tags
            .Include(t => t.QuestionTags)
            .AsNoTracking()
            .AsSplitQuery();

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            var search = queryParams.Search.ToLower();
            tags = tags.Where(t => t.Name.ToLower().Contains(search));
        }

        // `Popular` bo'yicha tartiblash
        if (queryParams.Popular.HasValue && queryParams.Popular == true)
        {
            tags = tags.OrderByDescending(t => t.QuestionTags.Count());
        }
        else
        {
            tags = tags.OrderBy(t => t.QuestionTags.Count());
        }

        if (queryParams.SortDescending.HasValue)
        {
            if (queryParams.SortDescending.Value)
            {
                tags = tags.OrderByDescending(t => t.Name); // Alifbo tartibida kamayish
            }
            else
            {
                tags = tags.OrderBy(t => t.Name); // Alifbo tartibida oshish
            }
        }

        var result = await tags.ToPaginatedList<TagDto, Tag>(_mapper.ConfigurationProvider, 1, 15);
        return result;
    }

    public async Task<Tag> GetTagByIdAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<TagDto> CreateTagAsync(TagForCreateDto tagForCreatedDto)
    {
        var entity = _mapper.Map<Tag>(tagForCreatedDto); 

        var result = _context.Tags.Add(entity).Entity;
        await _context.SaveChangesAsync();

        return _mapper.Map<TagDto>(result); ;
    }

    public async Task<Tag> UpdateTagAsync(int id, Tag tag)
    {
        var existingTag = await _context.Tags.FindAsync(id);
        if (existingTag == null)
        {
            return null;
        }

        existingTag.Name = tag.Name;

        await _context.SaveChangesAsync();
        return existingTag;
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            return false;
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }
}
