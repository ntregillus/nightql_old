

using System.ComponentModel.DataAnnotations;

namespace NightQL.Models
{
    /// <summary>
    /// defines a blue print for a relationship and if a child entity requires a parent record
    /// </summary>
    public class Relationship
    {
        /// <summary>
        /// identifies what type of record the relationship represents in the link table.
        /// </summary>
        public int RelationshipID { get; set; }

        /// <summary>
        /// alias describing what the parent entity represents to the child entity
        /// </summary>
        [Required, StringLength(50)]
        public string ParentAlias { get; set; }
        /// <summary>
        /// alias describing what the child entity represents to the parent entity
        /// </summary>
        [Required, StringLength(50)]
        public string ChildAlias {get;set;}
        /// <summary>
        /// Name of the parent entity this relationship is linking
        /// </summary>
        public string ParentEntity { get; set; }
        /// <summary>
        /// Name of the child entity this relationship is linking
        /// </summary>
        [Required, StringLength(50)]
        public string ChildEntity { get; set; }
        /// <summary>
        /// defines if a child record can be created without a parent record.
        /// </summary>
        [Required]
        public bool? ChildRequiresParent { get; set; }

    }

}