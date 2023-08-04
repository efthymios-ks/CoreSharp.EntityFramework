using CoreSharp.EntityFramework.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreSharp.EntityFramework.Entities.Abstracts;

public abstract class EntityBase<TKey> : EntityBase, IEntity<TKey>
{
    [Key]
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id
    {
        get => (this as IUniqueEntity).Id is TKey key ? key : default;
        set => (this as IUniqueEntity).Id = value;
    }

    // Methods 
    public override string ToString()
        => $"{Id}";
}
