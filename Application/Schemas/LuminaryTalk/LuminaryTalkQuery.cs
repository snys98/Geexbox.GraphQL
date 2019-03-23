using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using WebApplication2.Application.Schemas.LuminaryTalk.Outputs;
using WebApplication2.Infrastructure;

namespace WebApplication2.Application.Schemas.LuminaryTalk
{
    [ImplementViewer(OperationType.Query)]
    public class LuminaryTalkQuery
    {
        private readonly LuminaryTalkDbContext _dbContext;

        public LuminaryTalkQuery(LuminaryTalkDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Description("Retrieve book by its globally unique ID.")]
        public Task<List<LuminaryTalkOutput>> LuminaryTalk(List<string> ids) =>
            Task.FromResult(new[] { _dbContext.LuminaryTalks.First() }.Select(x => new LuminaryTalkOutput() { Title = x.Title }).ToList());
    }
}