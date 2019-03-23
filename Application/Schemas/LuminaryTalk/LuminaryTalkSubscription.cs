using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Conventions.Abstractions;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Relay;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using WebApplication2.Application.Schemas.LuminaryTalk.Outputs;
using WebApplication2.Infrastructure;

namespace WebApplication2.Application.Schemas.LuminaryTalk
{
    [ImplementViewer(OperationType.Subscription)]
    public class LuminaryTalkSubscription
    {
        private readonly LuminaryTalkDbContext _dbContext;

        public LuminaryTalkSubscription()
        {
            
        }
        public LuminaryTalkSubscription(LuminaryTalkDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IObservable<LuminaryTalkOutput> LuminaryTalkUpdated(ResolveEventStreamContext context)
        {
            ISubject<LuminaryTalkOutput> subject =
                new BehaviorSubject<LuminaryTalkOutput>(new LuminaryTalkOutput() { Title = "new title" });
            Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(3000);
                    subject.OnNext(new LuminaryTalkOutput() { Title = $"new title" + DateTime.Now });
                }
            });
            return subject.AsObservable();
        }
    }
}