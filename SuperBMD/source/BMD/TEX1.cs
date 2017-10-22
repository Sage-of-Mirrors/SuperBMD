using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials;
using GameFormatReader.Common;
using SuperBMD.Util;
using Assimp;

namespace SuperBMD.BMD
{
    public class TEX1
    {
        public List<BinaryTextureImage> Textures { get; private set; }

        public TEX1(EndianBinaryReader reader, int offset)
        {
            Textures = new List<BinaryTextureImage>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int tex1Size = reader.ReadInt32();
            short texCount = reader.ReadInt16();
            reader.SkipInt16();

            int textureHeaderOffset = reader.ReadInt32();
            int textureNameTableOffset = reader.ReadInt32();

            List<string> names = NameTableIO.Load(reader, offset + textureNameTableOffset);

            reader.BaseStream.Seek(textureHeaderOffset + offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < texCount; i++)
            {
                reader.BaseStream.Seek((offset + 0x20 + (0x20 * i)), System.IO.SeekOrigin.Begin);

                BinaryTextureImage img = new BinaryTextureImage();
                img.Load(reader, (offset + 0x20 + (0x20 * i)));
                Textures.Add(img);
            }
        }

        public TEX1(Assimp.Scene scene, string modelDirectory)
        {
            Textures = new List<BinaryTextureImage>();

            foreach (Assimp.Material mat in scene.Materials)
            {
                if (mat.HasTextureDiffuse)
                {
                    BinaryTextureImage img = new BinaryTextureImage();
                    img.Load(mat.TextureDiffuse, modelDirectory);
                    img.SaveImageToDisk($"D:\\SZS Tools\\SuperBMD\\textures\\{mat.Name}_tex.bmp");

                    Textures.Add(img);
                }
            }
        }

        public BinaryTextureImage this[int i]
        {
            get
            {
                if (Textures != null && Textures.Count > i)
                {
                    return Textures[i];
                }
                else
                {
                    Console.WriteLine($"Could not retrieve texture at index { i }.");
                    return null;
                }
            }
            set
            {
                if (Textures == null)
                    Textures = new List<BinaryTextureImage>();

                Textures[i] = value;
            }
        }
    }
}
