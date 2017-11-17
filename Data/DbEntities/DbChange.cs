using System;
using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbChange
    {
        public DbChange() {
            Guid = Guid.NewGuid();
        }
        public long ID { get; set; }
        
        /// <summary>
        /// used to identify a script across copied schemas
        /// </summary>
        public Guid Guid {get;set;}
        public DateTime CreatedTimeStamp {get;set;}
        [Required]
        public DbSchema Schema { get; set; }
        [StringLength(4000)]

        public string Forward { get; set; }
        [StringLength(4000)]
        public string Backward { get; set;}

        
    }
}