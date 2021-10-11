namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface IEntity<TKey> : IKeyedEntity<TKey>, IEntity
    {
        //Properties
        new TKey Id { get; set; }
    }
}
