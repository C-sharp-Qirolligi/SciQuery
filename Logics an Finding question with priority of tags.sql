select Count(qt.QuestionId),qt.QuestionId from Tag as t inner join QuestionTag as qt 
	on t.Id = qt.TagId where t.id >	10 group by qt.QuestionId order by Count(qt.QuestionId) desc;


