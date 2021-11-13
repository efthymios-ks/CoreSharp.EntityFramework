namespace CoreSharp.EntityFramework.Models.Interfaces
{
    /// <inheritdoc />
    public interface IKeyedEntity<TKey> : IKeyedEntity
    {
        //Properties 
        new TKey Id { get; set; }
    }
}
