using GraphQL.Types;
using WebApplication2.Domain;

namespace WebApplication2.Application.Types
{
    public class LuminaryTalkType : ObjectGraphType<LuminaryTalk>
    {
        public LuminaryTalkType()
        {
            Field(x => x.Title);
        }
    }
}