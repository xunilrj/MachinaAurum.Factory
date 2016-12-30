using MachinaAurum.Factory.LifetimeManagers;
using MachinaAurum.Factory.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachinaAurum.Factory
{
    class FactoryContext : IFactoryContext
    {
        FastDictionary Factories = new FastDictionary();
        Dictionary<Type, ILifetimeManager> LifetimeManagers = new Dictionary<Type, ILifetimeManager>();

        UserSpecifiedTypeSolver TypeSolver;

        public IFactory Factory { get; private set; }

        public FactoryContext(IFactory factory)
        {
            Factory = factory;

            TypeSolver = new UserSpecifiedTypeSolver();
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            object item;

            if (Factories.TryGetValue(type.GetHashCode(), out item) == false)
            {
                var lifeTimeManager = Factory.GetLifetimeManager(type);
                LifetimeManagers.Add(type, lifeTimeManager);

                var factory = lifeTimeManager.GetFactory(this, type);
                Factories.Add(type.GetHashCode(), factory);

                return factory();
            }

            return item;
        }

        public void Dispose()
        {
            foreach (var item in LifetimeManagers)
            {
                item.Value.Dispose();
            }
        }

        public void RegisterSingleton<T>(T instance)
        {
            //if (instance != null)
            //{
            //    TypeSolver.Register(typeof(T), instance.GetType());
            //}

            //var manager = new FactorySingletonLifetimeManager();
            //manager.Instance = instance;

            //LifetimeManagers.Add(typeof(T), manager);
        }

        public Type GetTypeFor(Type type)
        {
            var newType = TypeSolver.Solve(this.Factory, type);

            if (newType == null)
            {
                return Factory.GetImplementationType(type);
            }
            else
            {
                return newType;
            }
        }


        public IEnumerable<Tuple<Type, object>> GetTypesFor(Type type)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Tuple<Type, object>> GetTypesFor<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(T).IsAssignableFrom(x) && x.IsAbstract == false)
                .Select(x => Tuple.Create(x, x.GetCustomAttributes(true).First()));
        }


        public ILifetimeManager GetLifetime(object item)
        {
            return LifetimeManagers[item.GetType()];
        }
    }

    //class NamedFactoryContext : SimpleFactoryContext
    //{
    //    IDictionary<string, ILifetimeManager> NamedLifetimeManagers = new Dictionary<string, ILifetimeManager>();

    //    public NamedFactoryContext(IFactoryContext factoryContext)
    //        : base((factoryContext as SimpleFactoryContext).Factory, factoryContext)
    //    {
    //    }

    //    public override ILifetimeManager GetLifetimeManager<T>(string name, bool returnDefault)
    //    {
    //        ILifetimeManager manager = null;

    //        if (NamedLifetimeManagers.TryGetValue(name, out manager))
    //        {
    //            return manager;
    //        }

    //        return base.GetLifetimeManager<T>(name, returnDefault);
    //    }

    //    public void RegisterLifetime(string name, object value)
    //    {
    //        var singleton = new SingletonPerContextLifetimeManager();
    //        singleton.Instance = value;
    //        NamedLifetimeManagers.Add(name, singleton);
    //    }
    //}
}
