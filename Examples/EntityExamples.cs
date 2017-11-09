using NightQL.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NightQL.Examples
{
    internal class EntityExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Entity {
                Name = "Contact",
                Fields = new FieldList {
                    new Field
                    {
                        Name = "FirstName",
                        DataType = ValueType.String,
                        Length = 50,
                        Required = true
                    },
                    new Field
                    {
                        Name = "LastName",
                        DataType = ValueType.String,
                        Length = 50,
                        Required = true
                    },
                    new Field
                    {
                        Name = "Email",
                        DataType = ValueType.String,
                        Length = 50,
                        Required = false
                    },
                    new Field
                    {
                        Name = "BirthDate",
                        DataType = ValueType.Date,
                        Required = false
                    },
                    new Field {
                        Name = "Phone",
                        DataType = ValueType.String,
                        Required = false,
                        Length = 11
                    }
                }
            };
        }
    }

    internal class EntityListExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EntityList {
                new Entity {
                Name = "Contact",
                Fields = new FieldList {
                    new Field
                    {
                        Name = "FirstName",
                        DataType = ValueType.String,
                        Length = 50,
                        Required = true
                    },
                    new Field
                    {
                        Name = "LastName",
                        DataType = ValueType.String,
                        Length = 50,
                        Required = true
                    },
                    new Field
                    {
                        Name = "Email",
                        DataType = ValueType.String,
                        Length = 50,
                        Required = false
                    },
                    new Field
                    {
                        Name = "BirthDate",
                        DataType = ValueType.Date,
                        Required = false
                    },
                    new Field {
                        Name = "Phone",
                        DataType = ValueType.String,
                        Required = false,
                        Length = 11
                        }
                    }
                }
            };
        }
    }
}