using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.DTOs.Comment
{
    public class CommentForCreateDto
    {
        public string Body { get; set; }
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string UserId { get; set; }
    }

}
