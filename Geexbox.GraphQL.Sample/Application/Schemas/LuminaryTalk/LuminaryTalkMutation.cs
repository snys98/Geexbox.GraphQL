using System.Threading.Tasks;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk.Inputs;
using GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk.Outputs;
using GraphQL.Conventions.Sample.Infrastructure;
using Microex.All.Common;

namespace GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk
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