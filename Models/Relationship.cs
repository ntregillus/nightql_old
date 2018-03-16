

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using LinqKit;
using NightQL.Data.DbEntities;

namespace NightQL.Models
{
    /// <summary>
    /// defines a blue print for a relationship and if a child entity requires a parent record
    /// </summary>
    public class Relationship
    {

        /// <summary>
        /// alias describing what the parent entity represents to the child entity
        /// </summary>
        [Required, StringLength(50)]
        [RegularExpression("^[A-Za-z_][A-Za-z\\d_]*$", ErrorMessage ="Only alphanumeric characters are allowed")]
        public string ParentAlias { get; set; }
        /// <summary>
        /// alias describing what the child entity represents to the parent entity
        /// </summary>
        [Required, StringLength(50)]
        [RegularExpression("^[A-Za-z_][A-Za-z\\d_]*$", ErrorMessage ="Only alphanumeric characters are allowed")]
        public string ChildAlias {get;set;}
        /// <summary>
        /// Name of the parent entity this relationship is linking
        /// </summary>
        [Required, StringLength(50)]
        [RegularExpression("^[A-Za-z_][A-Za-z\\d_]*$", ErrorMessage ="Only alphanumeric characters are allowed")]
        public string ParentEntity { get; set; }
        /// <summary>
        /// Name of the child entity this relationship is linking
        /// </summary>
        [Required, StringLength(50)]
        [RegularExpression("^[A-Za-z_][A-Za-z\\d_]*$", ErrorMessage ="Only alphanumeric characters are allowed")]
        public string ChildEntity { get; set; }
        /// <summary>
        /// defines if a child record can be created without a parent record.
        /// </summary>
        [Required]
        public bool? ChildRequiresParent { get; set; }

        public int RequiresParent() {return ChildRequiresParent.Value ? 1 : 0; }

        public IEnumerable<DbChange> GetCreateScripts(string schema)
        {
            var fwd = new StringBuilder();
            fwd.AppendLine("INSERT INTO [dbo].[DbRelationship]([ChildAlias],[ChildEntity],[ChildRequiresParent],[ParentAlias],[ParentEntity],[SchemaID])");
            fwd.AppendLine($"SELECT '{ChildAlias}', '{ChildEntity}', {RequiresParent()}, '{ParentAlias}', '{ParentEntity}', s.ID");
            fwd.AppendLine($"FROM [dbo].[DbSchema] as s WHERE s.[Name] = SCHEMA_NAME()");
            var bkwrd = new StringBuilder();
            bkwrd.AppendLine("DELETE FROM [dbo].[Link] WHERE RelationshipID in ");
            bkwrd.AppendLine("(SELECT ID FROM [dbo].Relationship as r ");
            bkwrd.AppendLine(" JOIN [dbo].DbSchema as s on s.ID = r.SchemaID");
            bkwrd.AppendLine($" WHERE s.[Name] = SCHEMA_NAME()");
            bkwrd.AppendLine($" AND ChildAlias = '{ChildAlias}' AND ParentAlias = '{ParentAlias}';");
            bkwrd.AppendLine($"DELETE FROM [dbo].[Relationship]");
            bkwrd.AppendLine($" WHERE ChildAlias = '{ChildAlias}' AND ParentAlias = '{ParentAlias}'");
            bkwrd.AppendLine($" AND SchemaID in (SELECT ID from [dbo].[DbSchema] WHERE [Name] = SCHEMA_NAME());");
            yield return new DbChange 
            {
                Forward = fwd.ToString(),
                Backward = bkwrd.ToString()
            };
        }
        public bool MatchesAny(IQueryable<DbRelationship> list){
            var filter = PredicateBuilder.New<DbRelationship>();
            filter.And(r=>r.ChildAlias == ChildAlias);
            filter.And(r=>r.ParentAlias == ParentAlias);
            filter.And(r=>r.ParentEntity == ParentEntity);
            filter.And(r=>r.ChildEntity == ChildEntity);
            return list.Any(filter);
        }
    }

    public class RelationshipList : List<Relationship>, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
              foreach(var dupe in this.GroupBy(x=> (x.ParentAlias.ToLower(), x.ChildAlias))
              .Where(g=>g.Count()>1)
              .Select(y=> new { Relationship = y.First(), Counter = y.Count()}))
              {
                  yield return new ValidationResult($"More than one Relationship with the ParentAlias" + 
                  $" {dupe.Relationship.ParentAlias} and a childAlias of {dupe.Relationship.ChildAlias} was provided.");
              }
        }
    }

}