using MachinaAurum.Factory.Closures;
using MachinaAurum.Factory.LifetimeManagers;
using System;

namespace MachinaAurum.Factory
{
    public interface IFactory
    {
        void RegisterConvention(IConstructorResolutionConvention convention);

        void RegisterLifetime(Type T, ILifetimeManager manager);
        ILifetimeManager GetLifetimeManager(Type t);

        Type GetImplementationType(Type type);
        Tuple<Type, Type[]> GetClosureType(Type type);

        IFactoryContext CreateContext();
    }

    public static class FactoryExtensions
    {
        public static void RegisterLifetime<T>(this IFactory factory, ILifetimeManager manager)
        {
            factory.RegisterLifetime(typeof(T), manager);
        }
    }
}
