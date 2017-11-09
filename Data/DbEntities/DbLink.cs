using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbLink
    {
        public long ID { get; set; }
        public DbEntity Parent { get; set; }
        public DbEntity Child { get; set; }
        public DbRelationship Relationship { get; set; }
    }

}