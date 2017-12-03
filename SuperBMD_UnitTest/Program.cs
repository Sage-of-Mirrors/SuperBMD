using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD;

namespace SuperBMD_UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Model mod = Model.Load(args[0]);
            mod.Export(@"D:\SZS Tools\SuperBMD\test.bmd");
        }
    }
}
