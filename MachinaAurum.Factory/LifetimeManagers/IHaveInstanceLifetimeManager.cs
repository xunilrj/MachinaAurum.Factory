using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachinaAurum.Factory.LifetimeManagers
{
    public interface IHaveInstanceLifetimeManager : ILifetimeManager
    {
        object Instance { get; }        
    }
}
