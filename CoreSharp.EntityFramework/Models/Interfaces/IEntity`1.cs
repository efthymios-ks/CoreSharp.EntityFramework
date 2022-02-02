namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface IEntity<TKey> : IUniqueEntity<TKey>, IEntity
    {
        //Properties
        new TKey Id { get; set; }
    }
}
