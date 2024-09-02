using SciQuery.Domain.Entities;
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
        public int? PostId { get; set; }
        public PostType Post { get; set; }
        public string UserId { get; set; }
    }

}
