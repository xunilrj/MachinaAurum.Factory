using System;
using System.Linq;

namespace MachinaAurum.Factory.LifetimeManagers
{
    public class NewInstanceLifetimeManager : ILifetimeManager
    {
        Func<object> Create;

        public Func<object> GetFactory(IFactoryContext context, Type type)
        {
            if (Create == null)
            {
                var closure = context.Factory.GetClosureType(type);
                var closureInstance = Activator.CreateInstance(closure.Item1, closure.Item2.Select(x => context.Resolve(x)).ToArray());

                Create = (Func<object>)closureInstance.GetType().GetMethod("Build").CreateDelegate(typeof(Func<object>), closureInstance);
            }

            return Create;
        }

        public void Accept(Type type, IFactory factory)
        {
        }

        public void Dispose()
        {
        }
    }
}
