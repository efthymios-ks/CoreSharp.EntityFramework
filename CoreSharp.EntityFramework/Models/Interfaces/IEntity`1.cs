namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface IEntity<TKey> : IKeyedEntity<TKey>, ITrackableEntity
    {
    }
}
