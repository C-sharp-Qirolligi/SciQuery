using AutoMapper;
using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Question;

namespace SciQuery.Service.Mappings
{
    public class ForEasyQuestionMappings : Profile
    {
        public ForEasyQuestionMappings()
        {
            CreateMap<Question, ForEasyQestionDto>()
            .ForMember(dest => dest.AnswersCount, opt => opt.MapFrom(src => src.Answers.Count))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.QuestionTags.Select(qt => qt.Tag)));
        }
    }
}

