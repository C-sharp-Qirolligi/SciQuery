using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SciQuery.Domain.Entities;
using SciQuery.Domain.UserModels;
using System.Reflection.Emit;

namespace SciQuery.Infrastructure.Persistance.DbContext;

public class SciQueryDbContext(DbContextOptions<SciQueryDbContext> options,
    IConfiguration configuration) : IdentityDbContext<User>(options)
{
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<ReputationChange> ReputationChanges { get; set; }
    public virtual DbSet<QuestionTag> QuestionTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        builder.Entity<Answer>().ToTable(nameof(Answer));
        
        builder.Entity<Tag>().ToTable(nameof(Tag));
        
        builder.Entity<Question>().ToTable(nameof(Question));

        builder.Entity<QuestionTag>().ToTable(nameof(QuestionTag));
        
        builder.Entity<ReputationChange>().ToTable(nameof(ReputationChange));
        
        
        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnectionFirdavs"));
        base.OnConfiguring(optionsBuilder);
    }
}
