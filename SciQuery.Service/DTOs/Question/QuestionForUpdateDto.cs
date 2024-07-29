﻿using SciQuery.Domain.Entities;
using SciQuery.Service.DTOs.Answer;
using SciQuery.Service.DTOs.Comment;
using SciQuery.Service.DTOs.Vote;

namespace SciQuery.Service.DTOs.Question;

public class QuestionForUpdateDto
{
    public int Id { get; set; } 
    public string Title { get; set; }
    public string Body { get; set; }

}
