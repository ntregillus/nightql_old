using System;
using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbEntity
    {
        public long ID { get; set; }
        [Required]
        public DbSchema Schema {get;set;}
        public Guid Guid {get;set;}
        public DateTime CreatedTimeStamp {get;set;}
        public bool IsActive { get; set; }
    }
    
}