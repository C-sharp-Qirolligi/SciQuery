using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciQuery.Domain.Entities;
using SciQuery.Domain.Exceptions;
using SciQuery.Domain.UserModels;
using SciQuery.Infrastructure.Persistance.DbContext;
using SciQuery.Service.DTOs.Vote;
using SciQuery.Service.Interfaces;
using SciQuery.Service.Mappings.Extensions;
using SciQuery.Service.Pagination.PaginatedList;

namespace SciQuery.Service.Services;

public class VoteService(SciQueryDbContext context, IMapper mapper) : IVoteService
{
    private readonly SciQueryDbContext _context = context;
    private readonly IMapper _mapper = mapper;


    public async Task<PaginatedList<VoteDto>> GetAllVotesByQuestionIdAsync(int questionId)
    {
        var votes = await _context.Votes
            .Include(x => x.User)
            .Where(v => v.QuestionId == questionId)
            .AsNoTracking()
            .AsSplitQuery() 
            .ToPaginatedList<VoteDto, Vote>(_mapper.ConfigurationProvider);
        return votes;
    }
    public async Task<PaginatedList<VoteDto>> GetVoteByUserIdAsync(string userId)
    {
        var votes = await _context.Votes
            .Include(x => x.User)
            .Where(u => u.UserId == userId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToPaginatedList<VoteDto, Vote>(_mapper.ConfigurationProvider);

        return votes;
    }

    public async Task<PaginatedList<VoteDto>> GetAllVotesByAnswerIdAsync(int answerId)
    {
        var votes = await _context.Votes
            .Where(v => v.AnswerId == answerId)
            .AsNoTracking()
            .AsSplitQuery()
            .ToPaginatedList<VoteDto, Vote>(_mapper.ConfigurationProvider);
        return votes;
    }

    public async Task<VoteDto> CreateVoteAsync(VoteForCreateDto voteCreateDto)
    {
        var vote = _mapper.Map<Vote>(voteCreateDto);

        _context.Votes.Add(vote);
        await _context.SaveChangesAsync();

        return _mapper.Map<VoteDto>(vote);
    }

    public async Task<bool> DeleteVoteAsync(int id)
    {
        var vote = await _context.Votes.FindAsync(id)
            ?? throw new EntityNotFoundException($"Vote with id : {id} is not found!");

        _context.Votes.Remove(vote);
        await _context.SaveChangesAsync();
        return true;
    }
}
