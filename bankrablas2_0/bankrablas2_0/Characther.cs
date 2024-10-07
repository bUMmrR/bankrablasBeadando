using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    public abstract class Characther : VarosElem
    {
        public abstract int health { get; set; }

        public abstract int goldCount { get; set; }

    }
}
