namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface IKeyedEntity<TKey>
    {
        //Properties 
        TKey Id { get; set; }
    }
}
