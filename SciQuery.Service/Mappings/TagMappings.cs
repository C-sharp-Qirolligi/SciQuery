using AutoMapper;
using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Question;
using SciQuery.Service.DTOs.Tag;

namespace SciQuery.Service.Mappings;

public class TagMappings : Profile
{
    public TagMappings()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Questions, opt => opt
            .MapFrom(src => src.QuestionTags.Select(qt => qt.Question.Id)
            .Count()));
        CreateMap<TagForCreateAndUpdateDto, Tag>();
    }
}
