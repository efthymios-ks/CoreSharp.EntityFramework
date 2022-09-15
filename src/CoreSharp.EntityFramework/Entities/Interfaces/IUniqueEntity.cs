namespace CoreSharp.EntityFramework.Entities.Interfaces;

/// <summary>
/// Interface for entities with primary key.
/// </summary>
public interface IUniqueEntity
{
    // Properties 
    object Id { get; set; }
}
