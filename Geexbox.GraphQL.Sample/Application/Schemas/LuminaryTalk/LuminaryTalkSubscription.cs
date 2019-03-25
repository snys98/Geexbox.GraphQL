using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk.Outputs;
using GraphQL.Subscription;

namespace GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk
{
    [ImplementViewer(OperationType.Subscription)]
    public class LuminaryTalkSubscription
    {
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