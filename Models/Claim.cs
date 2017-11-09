namespace NightQL.Models
{
    /// <summary>
    /// Describes a persmission that can be assigned to an entity
    /// </summary>
    public class Claim
    {
        public string Entity { get; set; }
        public string Role { get; set; }

        public Action Actions { get; set; }
    }
    public class Action
    {
        public bool CanRead { get; set; }
        public int CanWrite { get; set; }
        public bool CanCreate { get; set; }
        public bool CanDelete { get; set; }
        public Ownership Owned { get; set; }
    }

    public enum Ownership
    {
        User,
        Role,
        All
    }
}