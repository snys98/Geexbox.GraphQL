using System;
using GraphQL.Conventions.Relay;

namespace GraphQL.Conventions.Sample.Application.Schemas.LuminaryTalk.Inputs
{

    public class CreateLuminaryTalkInput:IRelayMutationInputObject
    {
        [Ignore]
        public Domain.LuminaryTalk ToModel()
        {
            return new Domain.LuminaryTalk()
            {
                Id = Guid.NewGuid().ToString(),
                Title = this.Title
            };
        }

        public string Title { get; set; }
        public string ClientMutationId { get; set; }
    }
}