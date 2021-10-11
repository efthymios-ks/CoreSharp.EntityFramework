﻿namespace CoreSharp.EntityFramework.Models.Interfaces
{
    public interface IKeyedEntity<TKey> : IKeyedEntity
    {
        //Properties 
        new TKey Id { get; set; }
    }
}
