using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    public abstract class VarosElem
    {
        public abstract bool stepable { get;}

        public abstract bool seen { get; set; }

        public abstract override string ToString();



    }
}
