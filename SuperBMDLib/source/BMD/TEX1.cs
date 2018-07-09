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

                BinaryTextureImage img = new BinaryTextureImage(names[i]);
                img.Load(reader, (offset + 0x20 + (0x20 * i)));
                Textures.Add(img);
            }
        }

        public TEX1(Assimp.Scene scene, string modelDirectory)
        {
            Textures = new List<BinaryTextureImage>();

            foreach (Assimp.Mesh mesh in scene.Meshes)
            {
                Assimp.Material mat = scene.Materials[mesh.MaterialIndex];

                if (mat.HasTextureDiffuse)
                {
                    BinaryTextureImage img = new BinaryTextureImage();
                    img.Load(mat.TextureDiffuse, modelDirectory);
                    Textures.Add(img);
                }
            }
        }

        public void DumpTextures(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            foreach (BinaryTextureImage tex in Textures)
            {
                tex.SaveImageToDisk(directory);
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            long start = writer.BaseStream.Position;

            writer.Write("TEX1".ToCharArray());
            writer.Write(0); // Placeholder for section size
            writer.Write((short)Textures.Count);
            writer.Write((short)-1);
            writer.Write(32); // Offset to the start of the texture data. Always 32
            writer.Write(0); // Placeholder for string table offset

            StreamUtility.PadStreamWithString(writer, 32);

            List<string> names = new List<string>();
            List<Tuple<byte[], ushort[]>> imgData = new List<Tuple<byte[], ushort[]>>();

            foreach (BinaryTextureImage img in Textures)
            {
                imgData.Add(img.EncodeData());
                img.WriteHeader(writer);
                names.Add(img.Name);
            }

            long curOffset = writer.BaseStream.Position;

            // Palette pass
            for (int i = 0; i < imgData.Count; i++)
            {
                writer.Seek((int)start + (i * 32) + 44, System.IO.SeekOrigin.Begin); // offset of the image header + 32 bytes for the section's header + 12 bytes into the image header for palette offset
                writer.Write((int)(curOffset - start) - (32 + i * 32)); // Offsets are relative to the start of the image header
                writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

                if (imgData[i].Item2.Length > 0)
                {
                    foreach (ushort st in imgData[i].Item2)
                        writer.Write(st);

                    StreamUtility.PadStreamWithString(writer, 32);
                }

                curOffset = writer.BaseStream.Position;
            }

            // Image data pass
            for (int i = 0; i < imgData.Count; i++)
            {
                writer.Seek((int)start + (i * 32) + 60, System.IO.SeekOrigin.Begin); // offset of the image header + 32 bytes for the section's header + 28 bytes into the image header for image data offset
                writer.Write((int)(curOffset - start) - (32 + i * 32)); // Offsets are relative to the start of the image header
                writer.Seek(0, System.IO.SeekOrigin.End);

                writer.Write(imgData[i].Item1);

                curOffset = writer.BaseStream.Position;
            }

            writer.Seek((int)start + 16, System.IO.SeekOrigin.Begin);
            writer.Write((int)(curOffset - start));
            writer.Seek((int)curOffset, System.IO.SeekOrigin.Begin);

            NameTableIO.Write(writer, names);

            StreamUtility.PadStreamWithString(writer, 32);

            long end = writer.BaseStream.Position;
            long length = (end - start);

            writer.Seek((int)start + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)length);
            writer.Seek((int)end, System.IO.SeekOrigin.Begin);
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

        public BinaryTextureImage this[string s]
        {
            get
            {
                if (Textures == null)
                {
                    Console.WriteLine("There are no textures currently loaded.");
                    return null;
                }

                if (Textures.Count == 0)
                {
                    Console.WriteLine("There are no textures currently loaded.");
                    return null;
                }

                foreach (BinaryTextureImage tex in Textures)
                {
                    if (tex.Name == s)
                        return tex;
                }

                Console.WriteLine($"No texture with the name { s } was found.");
                return null;
            }

            private set
            {
                if (Textures == null)
                {
                    Textures = new List<BinaryTextureImage>();
                    Console.WriteLine("There are no textures currently loaded.");
                    return;
                }

                for (int i = 0; i < Textures.Count; i++)
                {
                    if (Textures[i].Name == s)
                    {
                        Textures[i] = value;
                        break;
                    }

                    if (i == Textures.Count - 1)
                        Console.WriteLine($"No texture with the name { s } was found.");
                }
            }
        }
    }
}
