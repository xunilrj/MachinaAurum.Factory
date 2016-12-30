using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MachinaAurum.Factory
{
    public interface IConstructorResolutionConvention
    {
        object Solve(IFactory factory, ParameterInfo info);
    }
}
