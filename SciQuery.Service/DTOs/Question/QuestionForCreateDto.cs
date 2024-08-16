using Microsoft.AspNetCore.Http;
using SciQuery.Service.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.DTOs.Question
{
    public class QuestionForCreateDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string? UserId { get; set; }
        public ICollection<string>? ImagePath { get; set; }
        public ICollection<string> Tags { get; set; }
    }

}
