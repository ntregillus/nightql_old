using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbRelationship
    {
        public int ID { get; set; }

        public DbSchema Schema {get; set;}

        [StringLength(128)]
        public string ParentEntity { get; set; }
        [StringLength(128)]
        public string ChildEntity { get; set; }
        [StringLength(128)]
        public string ParentAlias { get; set; }
        [StringLength(128)]
        public string ChildAlias { get; set; }
        public bool ChildRequiresParent { get; set; }
    }
}