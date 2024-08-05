using SciQuery.Service.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.DTOs.Question;

public class ForEasyQestionDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string UserId { get; set; }
    public UserDto User { get; set; }
    public int AnswersCount { get; set; }
    public int Votes { get; set; }
    public ICollection<string> Tags { get; set; }


}
