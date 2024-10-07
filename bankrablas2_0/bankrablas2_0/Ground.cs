using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    internal class Ground : VarosElem
    {
        public override bool stepable => true;

        private bool _seen;

        public override bool seen
        {
            get { return _seen; }
            set { _seen = value; }
        }

        public Ground()
        {
            seen = false;
        }

        public void OnStep()
        {
            return;

        }


        public override string ToString()
        {
            return (".");
        }

    }
}
