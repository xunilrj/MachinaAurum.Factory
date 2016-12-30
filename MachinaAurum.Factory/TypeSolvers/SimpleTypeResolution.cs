using System;

namespace MachinaAurum.Factory
{
    class InterfaceConventionSolver : ITypeSolver
    {
        public Type Solve(IFactory factory, Type type)
        {
            if (type.IsInterface)
            {
                return Type.GetType(string.Format("{0}.{1}, {2}", type.Namespace, type.Name.Substring(1, type.Name.Length - 1), type.Assembly.FullName));
            }

            return null;
        }
    }
}
