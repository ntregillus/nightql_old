using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NightQL.Data.DbEntities;

namespace NightQL.Models
{
    public class Schema: IValidatableObject
    {
        public Schema(){
            ReferenceSchemas = new List<string>();
        }
        /// <summary>
        /// the alias that will prefix all subsequent modifications to a schema
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

        /// <summary>
        /// used to dupliate an existing schema with a new name
        /// </summary>
        /// <returns></returns>
        public string CopiedSchema { get; set; }

        /// <summary>
        /// Schemas which can be used to extend the current schema
        /// </summary>
        /// <returns></returns>
        public List<string> ReferenceSchemas {get;set;}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var invalidNames = new string[] 
            {
                "dbo",
                "guest",
                "INFORMATION_SCHEMA",
                "sys",
                "db_owner",
                "db_accessadmin",
                "db_securityadmin",
                "db_ddladmin",
                "db_backupoperator",
                "db_datareader",
                "db_datawriter",
                "db_denydatareader",
                "db_denydatawriter"
            };
            if(invalidNames.Contains(Name))
            {
                yield return new ValidationResult($"Cannot use '{Name}' as Schema's Name.", new [] {"Name"});    
            }
            if(invalidNames.Contains(CopiedSchema))
            {
                yield return new ValidationResult($"Cannot use '{CopiedSchema}' as Schema's CopiedSchema.", new [] {"CopiedSchema"});    
            }
            for(var i = 0; i< ReferenceSchemas.Count;i++)
            {
                var refSchema = ReferenceSchemas[i];
                if(invalidNames.Contains(refSchema))
                {
                    yield return new ValidationResult($"Cannot use '{refSchema}' as a Schema's RefernceSchema.",
                     new [] {$"ReferenceSchema[{i}]"});    
                }
            }
        }

        public IEnumerable<DbChange> GetCreateScripts()
        {
            var createSchema = new DbChange {
                Forward = $"CREATE SCHEMA [{Name}]",
                Backward = $"DROP SCHEMA [{Name}]"
            };
            yield return createSchema;
            //TODO: add db changes to duplicate copiedschema
            //TODO: create method for adding views to referenced schemas

        }
    }
}