using MachinaAurum.Factory.Closures;
using System;
using System.Linq;

namespace MachinaAurum.Factory.LifetimeManagers
{
    public class FactorySingletonLifetimeManager : IHaveInstanceLifetimeManager
    {
        object _Instance;
        public object Instance { get { return _Instance; } }

        public Func<object> GetFactory(IFactoryContext context, Type type)
        {
            if (_Instance == null)
            {
                var closure = context.Factory.GetClosureType(type);
                var closureInstance = Activator.CreateInstance(closure.Item1, closure.Item2.Select(x => context.Resolve(x)).ToArray());
                _Instance = closure.Item1.GetMethod("Build").Invoke(closureInstance, null);
            }

            return () => _Instance;
        }

        public void Dispose()
        {
        }

        public void Accept(Type type, IFactory factory)
        {
            factory.RegisterLifetime(type, new FakeDisposableLifeTimeManager(this));
        }
    }

    public class FakeDisposableLifeTimeManager : IHaveInstanceLifetimeManager
    {
        FactorySingletonLifetimeManager Real;

        public FakeDisposableLifeTimeManager(FactorySingletonLifetimeManager real)
        {
            Real = real;
        }

        public object Instance
        {
            get { return Real.Instance; }
        }

        public Func<object> GetFactory(IFactoryContext context, Type type)
        {
            return Real.GetFactory(context, type);
        }

        public void Accept(Type type, IFactory factory)
        {
            Real.Accept(type, factory);
        }

        public void Dispose()
        {
        }
    }
}

namespace MachinaAurum.Factory
{
    public static class FactoryContextSingletonExtensions
    {
        public static void RegisterAsFactoryContextSingleton<T>(this IFactory factory)
        {
            //factory.RegisterLifetime<T, MachinaAurum.Factory.LifetimeManagers.SingletonPerContextLifetimeManager>();
        }
    }
}
