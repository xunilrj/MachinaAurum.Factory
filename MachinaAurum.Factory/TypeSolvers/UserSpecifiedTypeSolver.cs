using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaAurum.Factory
{
    class UserSpecifiedTypeSolver : ITypeSolver
    {
        IDictionary<Type, Type> Mapping = new Dictionary<Type, Type>();

        public Type Solve(IFactory factory, Type type)
        {
            Type specifiedType = null;

            if (Mapping.TryGetValue(type, out specifiedType))
            {
                return specifiedType;
            }

            return null;
        }

        public void Register(Type tfrom, Type tto)
        {
            Type specifiedType = null;

            if (Mapping.TryGetValue(tfrom, out specifiedType))
            {
                Mapping[tfrom] = tto;
            }
            else
            {
                Mapping.Add(tfrom, tto);
            }
        }
    }
}
