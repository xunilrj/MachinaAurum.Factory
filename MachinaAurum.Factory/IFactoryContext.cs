using MachinaAurum.Factory.LifetimeManagers;
using System;
using System.Collections.Generic;

namespace MachinaAurum.Factory
{
    public interface IFactoryContext : IDisposable
    {
        IFactory Factory { get; }

        object Resolve(Type type);
        T Resolve<T>();

        Type GetTypeFor(Type type);
        IEnumerable<Tuple<Type, object>> GetTypesFor<T>();

        ILifetimeManager GetLifetime(object item);
    }
}
