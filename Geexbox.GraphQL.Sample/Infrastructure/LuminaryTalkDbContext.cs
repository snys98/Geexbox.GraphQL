using GraphQL.Conventions.Sample.Domain;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Conventions.Sample.Infrastructure
{
    public class LuminaryTalkDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./luminarytalk.db");
        }

        public DbSet<LuminaryTalk> LuminaryTalks { get; protected set; }
    }
}