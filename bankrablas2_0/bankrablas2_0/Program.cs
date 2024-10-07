using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas2_0
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Varos varos = new Varos();

            //pálya létrhezozása, dolgok lerakása
            varos.init();

            //vilag vege inditasa
            varos.sim();


            Console.ReadLine();
        }
    }
}
