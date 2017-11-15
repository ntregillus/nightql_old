using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NightQL.Data.DbEntities;
using NightQL.Models;

namespace NightQL.Data
{
    public class ConfigContext: DbContext
    {
        public ConfigContext(DbContextOptions o):base(o){}

        public DbSet<DbSchema> Schemas {get;set;}
        public DbSet<DbEntity> Entities {get;set;}
        public DbSet<DbRelationship> Relationships {get;set;}
        public DbSet<DbLink> Links { get; set; }

        public DbSet<DbChange> Changes {get;set;}

        public DbSet<DbUser> Users {get;set;}

        public DbSet<DbPermission> Permissions {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbSchema>().ToTable("DbSchema");
            modelBuilder.Entity<DbEntity>().ToTable("DbEntity");
            modelBuilder.Entity<DbRelationship>().ToTable("DbRelationship");
            modelBuilder.Entity<DbLink>().ToTable("DbLink");
            modelBuilder.Entity<DbUser>().ToTable("DbUser");
            modelBuilder.Entity<DbChange>().ToTable("DbChange");
        }

        public IEnumerable<Entity> GetEntityDefinitions(string schema, EntitySearch query)
        {
            var exec = new SqlExcecuter(Database.GetDbConnection());
            var sql = new StringBuilder();
            sql.AppendLine(@"
            SELECT 
                TABLE_NAME as EntityName,
                COLUMN_NAME as FieldName,
                IS_NULLABLE as FieldRequired,
                CASE DATA_TYPE 
                WHEN 'nvarchar' THEN 'String'
                WHEN 'bigint'   THEN 'Long'
                WHEN 'varchar'	THEN 'String'
                WHEN 'int'		THEN 'Integer'
                WHEN 'Date' THEN 'Date'
                WHEN 'DateTime' THEN 'DateTime'
                WHEN 'DateTime2' THEN 'DateTime'
                WHEN 'bit' THEN 'Boolean'
                WHEN 'uniqueidentiifer' THEN 'Guid'
                ELSE 'UNKNOWN' END as DataType,
                CHARACTER_MAXIMUM_LENGTH as [Length]

            from INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @schema");
            exec.AddParameter("@schema", schema);
            if(!string.IsNullOrEmpty(query.Name))
            {
                sql.AppendLine("AND TABLE_NAME = @EntityName");
                exec.AddParameter("@EntityName", query.Name);

            }
            if(!string.IsNullOrWhiteSpace(query.FieldName))
            {
                sql.AppendLine("AND TABLE_NAME IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = @FieldName)");
                exec.AddParameter("@FieldName", query.FieldName);
            }
            if(!string.IsNullOrWhiteSpace(query.AssociatedEntity ))
            {
                sql.AppendLine("AND (TABLE_NAME IN (SELECT ChildEntity FROM Relationships WHERE ParentEntity = @AssoicatedEntity)");
                sql.AppendLine(" OR TABLE_NAME IN (SELECT ParentEntity FROM Relationships WHERE ChildEntity = @AssociatedEntity))");
                exec.AddParameter("@AssociatedEntity", query.AssociatedEntity);
            }
            var results = new Dictionary<string, Entity>();
            exec.ExecuteAsReader(sql.ToString(), (reader)=>
            {
                while (reader.Read())
                {
                    var name = reader["EntityName"].ToString();
                    Entity currentEntity = null;
                    if (!results.TryGetValue(name, out currentEntity))
                    {
                        currentEntity = new Entity { Name = name };
                        results.Add(name, currentEntity);
                    }
                    
                    var newField = new Field
                    {
                        Name = reader["FieldName"].ToString(),
                        Required = (reader["FieldRequired"].ToString() == "YES"),
                        Length = reader["Length"] == DBNull.Value ? null : new int?(Convert.ToInt32(reader["Length"]))
                    };
                    newField.SetValueTypeFromString(reader["DataType"].ToString());
                    currentEntity.Fields.Add(newField);
                }
            });
            return results.Values;
   
        }

        protected SqlExcecuter GetExcecuter(){
            return new SqlExcecuter(Database.GetDbConnection());
        }

        public void CreateEntity(string schema, Entity model)
        {
            var exec = GetExcecuter();
            SafelyExecuteChanges(model.GetCreateScripts(schema));
        }

        public async Task<Schema> GetSchemaAsync(string name) {
             var dbschema = await Schemas.FirstOrDefaultAsync(s => s.Name == name);
             if(dbschema == null) return null;
             return Mapper.Map<Schema>(dbschema);
        }

        public Schema CreateSchema(Schema model)
        {
            var dbSchema = Schemas.Add(new DbSchema{
                Name = model.Name
            });
            SafelyExecuteChanges(model.GetCreateScripts());
            // we do not need to create the schema as an entit yin the database, becuase
            // it doesn't exist without an entity to be contained within it.
            SaveChanges();
            return model;
        }

        public async Task CreateRelationships(string schema, RelationshipList relationships)
        {
            var dbSchema = await Schemas.FirstOrDefaultAsync(s=>s.Name == schema);
            
            var changes = relationships.SelectMany(r => 
                          r.GetCreateScripts(schema));
 
            SafelyExecuteChanges(changes);
        }

        protected void SafelyExecuteChanges(IEnumerable<DbChange> changes)
        {
            var successChanges = new Stack<DbChange>();
            try
            {
                foreach(var change in changes)
                {
                    Console.WriteLine("-----------------");
                    Console.WriteLine(change.Forward);
                    Database.ExecuteSqlCommand(change.Forward);
                    successChanges.Push(change);
                }
                Changes.AddRange(changes);
            }catch(Exception ex)
            {
                try{
                while(successChanges.Count > 0){
                    var rollback = successChanges.Pop();
                    Database.ExecuteSqlCommand(rollback.Backward);
                }
                }catch(Exception rollbackEx){
                    throw new AggregateException("Failed while running upgrade, then failed rolling back!", new [] {ex, rollbackEx});
                }
                throw;
            }

        }
    }
}