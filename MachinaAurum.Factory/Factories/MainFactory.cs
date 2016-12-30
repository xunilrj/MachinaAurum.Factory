using MachinaAurum.Factory.Closures;
using MachinaAurum.Factory.LifetimeManagers;
using System;
using System.Collections.Generic;

namespace MachinaAurum.Factory
{
    public abstract class BaseFactory : IFactory
    {
        ITypeSolver TypeSolver;
        UserSpecifiedTypeSolver UserSpecifiedTypeSolver;

        IDictionary<Type, ILifetimeManager> LifetimeManager = new Dictionary<Type, ILifetimeManager>();
        
        public BaseFactory()
        {
            UserSpecifiedTypeSolver = new UserSpecifiedTypeSolver();
            TypeSolver = new AggregatedTypeSolver(UserSpecifiedTypeSolver, new InterfaceConventionSolver());            
        }

        public void RegisterConvention(IConstructorResolutionConvention convention)
        {
            throw new NotImplementedException();
        }

        public Type GetImplementationType(Type type)
        {
            return TypeSolver.Solve(this, type);
        }

        public abstract Tuple<Type, Type[]> GetClosureType(Type type);

        public IFactoryContext CreateContext()
        {
            var childFactory = new ChildFactory(this);

            foreach (var manager in LifetimeManager)
            {
                manager.Value.Accept(manager.Key, childFactory);
            }

            var context = new FactoryContext(childFactory);
            
            return context;
        }
        
        public void RegisterImplementation<T1, T2>()
        {
            UserSpecifiedTypeSolver.Register(typeof(T1), typeof(T2));
        }

        public void RegisterLifetime(Type t, ILifetimeManager manager)
        {
            ILifetimeManager m;
            if (LifetimeManager.TryGetValue(t, out m))
            {
                LifetimeManager[t] = manager;
            }
            else
            {
                LifetimeManager.Add(t, manager);
            }
        }

        protected virtual ILifetimeManager GetDefaultLifetimeManager()
        {
            return null;
        }
        
        public virtual ILifetimeManager GetLifetimeManager(Type type)
        {
            ILifetimeManager manager = null;

            if (LifetimeManager.TryGetValue(type, out manager))
            {
                return manager;
            }
            else
            {
                return GetDefaultLifetimeManager();
            }
        }        
    }

    public class MainFactory : BaseFactory
    {
        ClosureManager ClosureManager;

        public MainFactory()
        {
            ClosureManager = new ClosureManager();
        }

        protected override ILifetimeManager GetDefaultLifetimeManager()
        {
            return new NewInstanceLifetimeManager();
        }

        public override Tuple<Type, Type[]> GetClosureType(Type type)
        {
            return ClosureManager.CreateType(this, type);
        }
    }
}
