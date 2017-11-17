using System;
using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbSchema
    {
        public int ID {get;set;}
        [Required]
        [StringLength(128)]
        public string Name {get;set;}
        public bool IsActive {get;set;}
        public DateTime CreatedTimeStamp { get; set;}
    }
}