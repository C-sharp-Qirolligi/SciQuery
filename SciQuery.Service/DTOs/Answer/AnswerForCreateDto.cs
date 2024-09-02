using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.DTOs.Answer
{
    public class AnswerForCreateDto
    {
        public string Body { get; set; }
        public int QuestionId { get; set; }
        public string UserId { get; set; }
        public List<string>? ImagePaths { get; set; }
    }
}
