namespace CoreSharp.EntityFramework.Models.Interfaces
{
    /// <summary>
    /// Interface for entities with primary key.
    /// </summary>
    public interface IKeyedEntity
    {
        //Properties 
        object Id { get; set; }
    }
}
