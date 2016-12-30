using System;

namespace MachinaAurum.Factory.LifetimeManagers
{
    public interface ILifetimeManager : IDisposable
    {
        Func<object> GetFactory(IFactoryContext context, Type type);

        void Accept(Type type, IFactory factory);
    }
}
