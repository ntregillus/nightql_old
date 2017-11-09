using System;
using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbEntity
    {
        public long ID { get; set; }
        public DbSchema Schema {get;set;}
        public Guid Guid {get;set;}
        
        public bool IsActive { get; set; }
    }
    
}