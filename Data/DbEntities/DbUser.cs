using System;
using System.ComponentModel.DataAnnotations;

namespace NightQL.Data.DbEntities
{
    public class DbUser
    {

        public int ID { get; set; }
        /// <summary>
        /// breif description of what these credentials are for
        /// </summary>
        [StringLength(128)]
        public string Description { get; set; }
        /// <summary>
        /// Authorization token to be provided for authorization and authentication
        /// </summary>
       [StringLength(128)]
       public string Secret { get; set; }

       public bool IsActive {get;set;}
       public DateTime CreatedTimeStamp {get;set;}
    }
}