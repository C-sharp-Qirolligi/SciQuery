using Bogus;
using SciQuery.Domain.Entities;
using SciQuery.Domain.UserModels;
using SciQuery.Domain.Votes;
using SciQuery.Infrastructure.Persistance.DbContext;

public class DatabaseSeeder
{
    public static void SeedData(SciQueryDbContext context)
    {
        try
        {
            AddUsers(context);
            context.SaveChanges();

            AddQuestions(context);
            context.SaveChanges();

            AddAnswers(context);
            context.SaveChanges();

            AddComments(context);
            context.SaveChanges();

            AddTags(context);
            context.SaveChanges();

            AddQuestionTags(context);
            context.SaveChanges();

            AddReputationChanges(context);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        } }

        private static void AddUsers(SciQueryDbContext context)
        {
            if (context.Users.Any()) return;
            // User generation
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.ProfileImagePath, f => f.Internet.Avatar())
                .RuleFor(u => u.Reputation, f => f.Random.Int(0, 1000))
                .RuleFor(u => u.CreatedDate, f => f.Date.Past(5))
                .RuleFor(u => u.LastLogindate, f => f.Date.Recent());

            var users = userFaker.Generate(10); // 10 users
            context.Users.AddRange(users);
        }

        private static void AddQuestions(SciQueryDbContext context)
        {
            if (context.Questions.Any()) return;
            var userIds = context.Users.Select(u => u.Id).ToList(); // Get user IDs

            var questionFaker = new Faker<Question>()
                .RuleFor(q => q.UserId, f => f.PickRandom(userIds))
                .RuleFor(q => q.Title, f => f.Lorem.Sentence())
                .RuleFor(q => q.Body, f => f.Lorem.Paragraphs())
                .RuleFor(q => q.CreatedDate, f => f.Date.Past())
                .RuleFor(q => q.UpdatedDate, f => f.Date.Past());

            var questions = questionFaker.Generate(50); // 20 questions
            context.Questions.AddRange(questions);
        }

        private static void AddAnswers(SciQueryDbContext context)
        {
            if (context.Answers.Any()) return;
            var questionIds = context.Questions.Select(q => q.Id).ToList();
            var userIds = context.Users.Select(u => u.Id).ToList();

            var answerFaker = new Faker<Answer>()
                .RuleFor(a => a.QuestionId, f => f.PickRandom(questionIds))
                .RuleFor(a => a.UserId, f => f.PickRandom(userIds))
                .RuleFor(a => a.Body, f => f.Lorem.Paragraphs())
                .RuleFor(a => a.CreatedDate, f => f.Date.Past())
                .RuleFor(a => a.UpdatedDate, f => f.Date.Past());

            var answers = answerFaker.Generate(50); // 50 answers
            context.Answers.AddRange(answers);
        }

        private static void AddComments(SciQueryDbContext context)
        {
            if (context.Comments.Any()) return;
            var userIds = context.Users.Select(u => u.Id).ToList();
            var questionIds = context.Questions.Select(q => q.Id).ToList();
            var answerIds = context.Answers.Select(a => a.Id).ToList();

            var commentFaker = new Faker<Comment>()
                .RuleFor(c => c.UserId, f => f.PickRandom(userIds))
                .RuleFor(c => c.PostId, f => f.PickRandom(questionIds))
                .RuleFor(c => c.Body, f => f.Lorem.Paragraphs())
                .RuleFor(c => c.CreatedDate, f => f.Date.Past());

            var commentFakerAnswer = new Faker<Comment>()
                .RuleFor(c => c.UserId, f => f.PickRandom(userIds))
                .RuleFor(c => c.PostId, f => f.PickRandom(answerIds))
                .RuleFor(c => c.Body, f => f.Lorem.Paragraphs())
                .RuleFor(c => c.CreatedDate, f => f.Date.Past());

            var comments = commentFaker.Generate(100); // 100 comments
            var commentsAnswer = commentFakerAnswer.Generate(100); // 100 comments

            for (int i = 0; i < 100; i++)
            {
                comments[i].Post = PostType.Question;
                commentsAnswer[i].Post = PostType.Answer;
            }

            context.Comments.AddRange(comments);
            context.Comments.AddRange(commentsAnswer);
        }

        private static void AddTags(SciQueryDbContext context)
        {
            if (context.Tags.Any()) return;
            var tagFaker = new Faker<Tag>()
                .RuleFor(t => t.Name, f => f.Lorem.Word());

            var tags = tagFaker.Generate(50); // 15 tags
            context.Tags.AddRange(tags);
        }

        private static void AddQuestionTags(SciQueryDbContext context)
        {
            if (context.QuestionTags.Any()) return;
            var questionIds = context.Questions.Select(q => q.Id).ToList();
            var tagIds = context.Tags.Select(t => t.Id).ToList();

            var questionTagFaker = new Faker<QuestionTag>()
                .RuleFor(qt => qt.QuestionId, f => f.PickRandom(questionIds))
                .RuleFor(qt => qt.TagId, f => f.PickRandom(tagIds));

            var questionTags = questionTagFaker.Generate(100); // 50 question tags
            context.QuestionTags.AddRange(questionTags);
        }

        private static void AddReputationChanges(SciQueryDbContext context)
        {
            if (context.ReputationChanges.Any()) return;
            var userIds = context.Users.Select(u => u.Id).ToList();

            var reputationChangeFaker = new Faker<ReputationChange>()
                .RuleFor(rc => rc.UserId, f => f.PickRandom(userIds))
                .RuleFor(rc => rc.ChangeAmount, f => f.Random.Int(-100, 100))
                .RuleFor(rc => rc.Reason, f => f.Lorem.Sentence())
                .RuleFor(rc => rc.CreatedDate, f => f.Date.Past());

            var reputationChanges = reputationChangeFaker.Generate(30); // 30 ta ma'lumot
            context.ReputationChanges.AddRange(reputationChanges);
        }
    } 
