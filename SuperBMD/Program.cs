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
            //SuperBMD.Materials.BinaryTextureImage img = new Materials.BinaryTextureImage();
            //img.LoadImageFromDisk(@"D:\SZS Tools\SuperBMD\succ.png");

            //using (FileStream strm = new FileStream(@"D:\SZS Tools\SuperBMD\test.bti", FileMode.Create, FileAccess.Write))
            //{
                //EndianBinaryWriter writer = new EndianBinaryWriter(strm, Endian.Big);
                //img.WriteHeader(writer);
                //writer.Write(img.EncodeData(Materials.BinaryTextureImage.TextureFormats.RGBA32));
            //}

            //Assimp.Scene test = LoadAssimpScene(args[0]);
            //JNT1 testJnt = new JNT1(test);
            //EVP1 testEvp = new EVP1(test, testJnt.FlatSkeleton);

            using (FileStream str = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            {
                EndianBinaryReader reader = new EndianBinaryReader(str, Endian.Big);
                Model testModel = new Model(reader);
            }
        }

        static Assimp.Scene LoadAssimpScene(string fileName)
        {
            Assimp.AssimpContext cont = new Assimp.AssimpContext();

            // AssImp adds dummy nodes for pivots from FBX, so we'll force them off
            cont.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
            cont.ZAxisRotation = -90.0f;
            return cont.ImportFile(fileName, Assimp.PostProcessSteps.Triangulate);
        }
    }
}
