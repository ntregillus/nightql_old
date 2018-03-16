using System;
using System.ComponentModel.DataAnnotations;
namespace NightQL.Data.DbEntities
{
    public class DbRelationship
    {
        public int ID { get; set; }
        [Required]
        public DbSchema Schema {get; set;}
        [Required]
        [StringLength(128)]
        public string ParentEntity { get; set; }
        [Required]
        [StringLength(128)]
        public string ChildEntity { get; set; }
        [Required]
        [StringLength(128)]
        public string ParentAlias { get; set; }
        [Required]
        [StringLength(128)]
        public string ChildAlias { get; set; }
        public bool ChildRequiresParent { get; set; }
        public bool IsActive {get;set;}
        public DateTime CreatedTimeStamp {get;set;}
    }
}