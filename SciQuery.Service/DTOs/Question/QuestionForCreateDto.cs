﻿using System;
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
        public int UserId { get; set; }
    }

}
