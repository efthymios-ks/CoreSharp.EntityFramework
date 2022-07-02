namespace CoreSharp.EntityFramework.Entities.Interfaces;

public interface IEntity<TKey> : IUniqueEntity<TKey>, IEntity
{
    //Properties
    new TKey Id { get; set; }
}
