using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk.Outputs;
using GraphQL.Conventions.Sample.Infrastructure;
using GraphQL.Types;

namespace GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk
{
    [ImplementViewer(OperationType.Query)]
    public class LuminaryTalkQuery
    {
        private LuminaryTalkDbContext _dbContext;

        public LuminaryTalkQuery(LuminaryTalkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Description("Retrieve book by its globally unique ID.")]
        public Task<List<LuminaryTalkOutput>> LuminaryTalk(List<string> ids)
        {
            return Task.FromResult(
                new[]
                {
                    _dbContext.LuminaryTalks.First()
                }.Select(x => new LuminaryTalkOutput() { Title = x.Title }).ToList());
        }
    }
}