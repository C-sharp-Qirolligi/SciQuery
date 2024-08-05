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
    public virtual DbSet<Vote> Votes { get; set; }
    public virtual DbSet<ReputationChange> ReputationChanges { get; set; }
    public virtual DbSet<QuestionTag> QuestionTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);

            entity.HasMany(q => q.QuestionTags)
                  .WithOne(qt => qt.Question)
                  .HasForeignKey(qt => qt.QuestionId);

            entity.HasMany(q => q.Answers)
                  .WithOne(a => a.Question)
                  .HasForeignKey(a => a.QuestionId);

            entity.HasMany(q => q.Comments)
                  .WithOne(c => c.Question)
                  .HasForeignKey(c => c.QuestionId);

            entity.HasMany(q => q.Votes)
                  .WithOne(v => v.Question)
                  .HasForeignKey(v => v.QuestionId);
        });

        builder.Entity<QuestionTag>(entity =>
        {
            entity.HasKey(qt => qt.Id);

            entity.HasOne(qt => qt.Question)
                  .WithMany(q => q.QuestionTags)
                  .HasForeignKey(qt => qt.QuestionId);

            entity.HasOne(qt => qt.Tag)
                  .WithMany(t => t.QuestionTags)
                  .HasForeignKey(qt => qt.TagId);
        });

        builder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
        });

        builder.Entity<Comment>().ToTable(nameof(Comment));

        builder.Entity<Answer>().ToTable(nameof(Answer));
        
        builder.Entity<Tag>().ToTable(nameof(Tag));
        
        builder.Entity<Vote>().ToTable(nameof(Vote));

        builder.Entity<Question>().ToTable(nameof(Question));

        builder.Entity<QuestionTag>().ToTable(nameof(QuestionTag));
        
        builder.Entity<ReputationChange>().ToTable(nameof(ReputationChange));
        
        
        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnectionJavlon"));
        base.OnConfiguring(optionsBuilder);
    }
}
