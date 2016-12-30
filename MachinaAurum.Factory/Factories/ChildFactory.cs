using MachinaAurum.Factory.LifetimeManagers;
using System;
using System.Collections.Generic;

namespace MachinaAurum.Factory
{
    public class ChildFactory : BaseFactory
    {
        IFactory Parent;

        public ChildFactory(IFactory parent)
        {
            Parent = parent;
        }

        public override Tuple<Type, Type[]> GetClosureType(Type type)
        {
            return Parent.GetClosureType(type);
        }

        public override ILifetimeManager GetLifetimeManager(Type type)
        {
            var manager = base.GetLifetimeManager(type);

            if (manager == null)
            {
                return Parent.GetLifetimeManager(type);
            }
            else
            {
                return manager;
            }
        }
    }
}
