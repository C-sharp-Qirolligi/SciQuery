using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.DTOs.User;

namespace SciQuery.Service.DTOs.Answer
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public string? Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<string>? ImagePaths { get; set; }
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public UserDto User { get; set; }

        public int Votes { get; set; } = 0;
        public ICollection<CommentDto> Comments { get; set; }
        public ICollection<ImageFile> Images { get; set; }
        public AnswerDto()
        {
            Images = new List<ImageFile>();
            Comments = new List<CommentDto>();
            Images = new List<ImageFile>();
        }
    }
}
