using System;
using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbLink
    {
        public long ID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedTimeStamp {get;set;}
        [Required]
        public DbEntity Parent { get; set; }
        [Required]
        public DbEntity Child { get; set; }
        [Required]
        public DbRelationship Relationship { get; set; }
    }

}