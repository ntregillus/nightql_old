using NightQL.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NightQL.Examples
{
    internal class RelationshipExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Relationship {
                ParentAlias = "Business",
                ChildAlias = "Owner",
                ParentEntity = "Business",
                ChildEntity = "Contact",
                ChildRequiresParent = true
            };
        }
    }

    internal class RelationshipListExamples:IExamplesProvider
    {
        public object GetExamples()
        {
            return new [] {
                new Relationship {
                    ParentAlias = "Business",
                    ChildAlias = "Owner",
                    ParentEntity = "Business",
                    ChildEntity = "Contact",
                    ChildRequiresParent = true
                },
                new Relationship 
                {
                    ParentAlias = "Contact",
                    ChildAlias = "MailingAddress",
                    ParentEntity = "Contact",
                    ChildEntity = "Address"
                }
                
            };
        }

    }

}