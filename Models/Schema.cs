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
        /// Authentication value to use in the auth header
        /// </summary>
        /// <returns></returns>
        public string Secret {get;set;}

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
            var createLogin = new DbChange {
                Forward = $"CREATE LOGIN [user_{Name}] WITH PASSWORD = 'Nate5462{Name}';",
                Backward = $"DROP LOGIN [user_{Name}];"
            };
            yield return createLogin;
            var createUser = new DbChange {
                Forward = $"CREATE USER [user_{Name}] FOR LOGIN [user_{Name}] WITH DEFAULT_SCHEMA = [{Name}]",
                Backward = $"DROP USER IF EXISTS [user_{Name}]"
            };
            yield return createUser;
            var denyPermissions = new DbChange {
                Forward = $"DENY EXECUTE ON SCHEMA:: [{Name}] TO [user_{Name}]",
                Backward = $"REVOKE EXECUTE ON SCHEMA:: [{Name}] TO [user_{Name}]"
            };
            yield return denyPermissions; //removes permissions to procs entirly 
            var grantPermissions = new DbChange {
                Forward = $"GRANT CONTROL ON SCHEMA:: [{Name}] TO [user_{Name}]",
                Backward = $"REVOKE CONTROL ON SCHEMA:: [{Name}] TO [user_{Name}]"
            };
            yield return grantPermissions;
            var grantCreateTable = new DbChange {
                Forward = $"GRANT CREATE TABLE TO [user_{Name}]",
                Backward = $"REVOKE CREATE TABLE TO [user_{Name}]"
            };
            yield return grantCreateTable;
            var grantDboPermissions = new DbChange {
                Forward = $"GRANT REFERENCES, SELECT, INSERT, UPDATE ON DbEntity TO [user_{Name}]",
                Backward = $"REVOKE REFERENCES, SELECT, INSERT, UPDATE ON DbEntity TO [user_{Name}]"
            };
            yield return grantCreateTable;
            var grantSelectDbSchema = new DbChange {
                Forward = $"GRANT SELECT ON DbSchema TO [user_{Name}]",
                Backward = $"REVOKE SELECT ON DbSchema TO [user_{Name}]"
            };
            yield return grantSelectDbSchema;            
            var grantDbRelationship = new DbChange {
                Forward = $"GRANT SELECT, INSERT, DELETE ON DbRelationship TO [user_{Name}]",
                Backward = $"REVOKE SELECT, INSERT, DELETE ON DbRelationship TO [user_{Name}]"
            };
            yield return grantSelectDbSchema;
            //TODO: add db changes to duplicate copiedschema
            //TODO: create method for adding views to referenced schemas

        }
    }
}