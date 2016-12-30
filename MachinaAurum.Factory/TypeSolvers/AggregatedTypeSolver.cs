using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaAurum.Factory
{
    class AggregatedTypeSolver : ITypeSolver
    {
        ITypeSolver[] Solvers;

        public AggregatedTypeSolver(params ITypeSolver[] solvers)
        {
            Solvers = solvers;
        }

        public Type Solve(IFactory factory, Type type)
        {
            foreach (var item in Solvers)
            {
                var result = item.Solve(factory, type);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
