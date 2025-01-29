﻿using CoreSharp.EntityFramework.Stores.Interfaces;
using CoreSharp.EntityFramework.Tests.Internal.Database.Models;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.Stores;

public interface IExtendedDummyStore : IExtendedStore<DummyEntity, Guid>
{
}

