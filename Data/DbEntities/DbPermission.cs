using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbPermission
    {
        public long ID { get; set; }
        public DbUser User { get; set; }
        [StringLength(128)]
        public string SchemaName { get; set; }
        /// <summary>
        /// if null, then permissions are for the entire schema
        /// </summary>
        /// <remarks>if set, then this set of permissions are for a single entity type
        /// <returns></returns>
        [StringLength(128)]
        public string Entity {get;set;}
        public bool ReadAccess { get; set; }
        public bool WriteAccess { get; set; }
        public bool CreateAccess { get; set; }
    }

}