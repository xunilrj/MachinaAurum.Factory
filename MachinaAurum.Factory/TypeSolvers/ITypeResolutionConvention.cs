using System;

namespace MachinaAurum.Factory
{
    interface ITypeSolver
    {
        Type Solve(IFactory factory, Type type);
    }
}
