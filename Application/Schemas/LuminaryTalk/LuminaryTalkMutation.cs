using System.Threading.Tasks;
using AutoMapper;
using GraphQL.Conventions.Relay;
using Microex.All.Common;
using WebApplication2.Application.Schemas.LuminaryTalk.Inputs;
using WebApplication2.Application.Schemas.LuminaryTalk.Outputs;
using WebApplication2.Infrastructure;

namespace WebApplication2.Application.Schemas.LuminaryTalk
{
    [ImplementViewer(OperationType.Mutation)]
    public class LuminaryTalkMutation
    {
        private readonly LuminaryTalkDbContext _dbContext;

        public LuminaryTalkMutation(LuminaryTalkDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<LuminaryTalkOutput> CreateLuminaryTalk(CreateLuminaryTalkInput input)
        {

            var luminaryTalk = input.ToModel();
            _dbContext.LuminaryTalks.Add(luminaryTalk);
            _dbContext.SaveChanges();
            _dbContext.Entry(luminaryTalk).Reload();
            return luminaryTalk.MapTo(new LuminaryTalkOutput());
        }
    }
}