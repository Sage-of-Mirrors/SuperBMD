using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.BMD;
using GameFormatReader.Common;
using System.IO;

namespace SuperBMD
{
    class Program
    {
        static void Main(string[] args)
        {
            //Assimp.Scene test = LoadAssimpScene(args[0]);

            using (FileStream str = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            {
                EndianBinaryReader reader = new EndianBinaryReader(str, Endian.Big);

                //VTX1 test = new VTX1(reader, 0x100);
                MAT3 testMat = new MAT3(reader, 0x5B00);
            }
        }

        static Assimp.Scene LoadAssimpScene(string fileName)
        {
            Assimp.AssimpContext cont = new Assimp.AssimpContext();
            return cont.ImportFile(fileName, Assimp.PostProcessSteps.Triangulate);
        }
    }
}
