using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NightQL.Data.DbEntities;

namespace NightQL.Models
{
    /// <summary>
    /// Describes a persistent type of record that will have an endpoint in the API
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The name of the persistent entity. It will be the Operation that will appear in the api
        /// </summary>
        /// <returns></returns>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// the list of claims that will allow edits to this entity
        /// </summary>
        /// <returns></returns>
        public List<Claim> Claims { get; set; }

        public List<Field> Fields { get; set; }

        public IEnumerable<DbChange> GetCreateScripts(string schema)
        {
            var createRawTable = new DbChange();
            var script = new StringBuilder();
            script.AppendLine($"CREATE TABLE [{schema}].[{Name}] (");
            script.AppendLine(" ID BIGINT NOT NULL,");
            var writeColumns = string.Join(", "+Environment.NewLine, from field in Fields 
                                select field.MsSqlCreate());
            script.AppendLine(writeColumns);
            script.AppendLine($", CONSTRAINT PK_{Name} PRIMARY KEY (ID)");
            script.AppendLine(")");

            if(script.Length > 4000){
                // we need to split this create into more than one insert....
                // TODO: write this algorithm! 
                throw new NotImplementedException("Too many columns. create table exceeds 4000 character limit!");
            }

            createRawTable.Forward = script.ToString();
            createRawTable.Backward = $"DROP TABLE [{schema}].[{Name}]";
            yield return createRawTable;
            //next foriegn keys
            var fkChange = new DbChange();
            fkChange.Forward = $"ALTER TABLE [{schema}].[{Name}]  WITH CHECK ADD  CONSTRAINT [FK_DbEntity_{schema}_{Name}] FOREIGN KEY([ID])"
                             + $"REFERENCES [dbo].[DbEntity] ([ID])";
            fkChange.Backward = $"ALTER TABLE [{schema}].[{Name}] DROP CONSTRAINT [FK_DbEntity_{schema}_{Name}]";
            yield return fkChange;

        }
    }

    public class EntityList : List<Entity>, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach(var dupe in this.GroupBy(x=>x.Name.ToLower())
              .Where(g=>g.Count()>1)
              .Select(y=> new { Entity = y.First(), Counter = y.Count()}))
              {
                  yield return new ValidationResult($"More than one entity with the name {dupe.Entity.Name} was provided.");
              }
              
        }
    }
}