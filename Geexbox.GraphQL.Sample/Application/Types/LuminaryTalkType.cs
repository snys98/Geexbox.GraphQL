using GraphQL.Conventions.Sample.Domain;
using GraphQL.Types;

namespace GraphQL.Conventions.Sample.Application.Types
{
    public class LuminaryTalkType : ObjectGraphType<LuminaryTalk>
    {
        public LuminaryTalkType()
        {
            Field(x => x.Title);
        }
    }
}