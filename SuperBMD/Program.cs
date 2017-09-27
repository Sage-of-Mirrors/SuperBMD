using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimp.Scene test = LoadAssimpScene(args[0]);
        }

        static Assimp.Scene LoadAssimpScene(string fileName)
        {
            Assimp.AssimpContext cont = new Assimp.AssimpContext();
            return cont.ImportFile(fileName, Assimp.PostProcessSteps.Triangulate);
        }
    }
}
