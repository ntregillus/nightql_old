using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NightQL.Models
{
    /// <summary>
    /// defines a field within an Entity
    /// </summary>
    public class Field
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public ValueType? DataType { get; set; }
        [Range(1, 4000)]
        public int? Length { get; set; }
        [Required]
        public bool? Required { get; set; }

        public string MsSqlCreate()
        {
            return $"{Name} {dbType} {isNull}";
        }

        protected string dbType{
            get{
                string result = null;
                switch(DataType.Value)
                {
                    case ValueType.Boolean:
                    result = "[bit]";
                    break;
                    case ValueType.Date:
                    result = "[date]";
                    break;
                    case ValueType.DateTime:
                    result = "[datetime2]";
                    break;
                    case ValueType.Guid:
                    result = "[uniqueidentifier]";
                    break;
                    case ValueType.Integer:
                    result = "[int]";
                    break;
                    case ValueType.String:
                    result = $"[nvarchar]({Length})";
                    break;
                    default:
                    throw new NotImplementedException($"not handling valuetype of {DataType}.");
                }
                return result;
            }
        }
        protected string isNull {
            get{
                if(Required.Value){
                    return "NOT NULL";
                }
                return "NULL";
            }
        }
    }
    public enum ValueType
    {
        Boolean,
        Integer,
        String,
        Date,
        DateTime,
        Guid
    }
    public class FieldList : List<Field>, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
         foreach(var dupe in (this).GroupBy(x => x.Name.ToLower())
              .Where((IGrouping<string, Field> g) => g.Count()>1)
              .Select((IGrouping<string, Field> y)=> new { Field = y.First(), Counter = y.Count()}))
              {
                  yield return new ValidationResult($"More than one Field with the name {dupe.Field.Name} was provided.");
              }
        }
    }

}