namespace SciQuery.Service.Interfaces;

public interface IReputationService
{
    Task CreateQuestionReputation(string userId);
    Task CreateAnswerReputation(string userId);
    Task AcceptedAnswerReputation(string userId);
    Task UpVotedAnswerReputation(string userId);
    Task DownVotedAnswerReputation(string userId);
    Task UpVotedQuestionReputation(string userId);
    Task DownVotedQuestionReputation(string userId);

}
