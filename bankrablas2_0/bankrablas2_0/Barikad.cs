﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    internal class Barikad : VarosElem
    {
        public override bool stepable => false;

        private bool _seen;

        public override bool seen
        {
            get { return _seen; }
            set { _seen = value; }
        }

        public Barikad()
        {
            seen = seen;
        }

        public void OnStep()
        {
            return;
        }


        public override string ToString()
        {
            return ("X");
        }
    }
}
