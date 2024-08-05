using AutoMapper;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Votes;
using SciQuery.Service.DTOs.Question;

namespace SciQuery.Service.Mappings
{
    public class QuestionMappings : Profile
    {
        public QuestionMappings()
        {
            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.Tags, opt => opt
                .MapFrom(src => src.QuestionTags.Select(qt => qt.Tag.Name)
                .ToList()));

            CreateMap<QuestionForCreateDto, Question>();
            CreateMap<QuestionForUpdateDto, Question>();
        }
    }
}
