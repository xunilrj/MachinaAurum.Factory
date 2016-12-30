using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinaAurum.Factory.Utils
{
    public class FastDictionary
    {
        Func<object>[] Values;
        int Lenght;

        public FastDictionary()
        {
            Lenght = 1000;
            Values = new Func<object>[Lenght];
        }

        public void Add(int key, Func<object> value)
        {
            Values[key % Lenght] = value;
        }

        public bool TryGetValue(int key, out object value)
        {
            var f = Values[key % Lenght];

            if (f == null)
            {
                value = null;
                return false;
            }

            value = f();
            return true;
        }
    }
}
