using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbChange
    {
        public long ID { get; set; }
        public DbSchema Schema { get; set; }
        [StringLength(4000)]

        public string Forward { get; set; }
        [StringLength(4000)]
        public string Backward { get; set;}
    }

}