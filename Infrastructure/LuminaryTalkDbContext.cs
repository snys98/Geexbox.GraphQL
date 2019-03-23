using Microsoft.EntityFrameworkCore;
using WebApplication2.Domain;

namespace WebApplication2.Infrastructure
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