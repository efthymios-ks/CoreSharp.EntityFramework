namespace CoreSharp.EntityFramework.Entities.Interfaces
{
    /// <inheritdoc cref="IUniqueEntity"/>
    public interface IUniqueEntity<TKey> : IUniqueEntity
    {
        //Properties 
        new TKey Id { get; set; }
    }
}
