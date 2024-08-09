using AutoMapper;
using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Comment;

namespace SciQuery.Service.Mappings
{
    public class CommentMappings : Profile
    {
        public CommentMappings()
        {
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentForCreateDto, Comment>();
            CreateMap<CommentForUpdateDto, Comment>();

        }
    }
}
