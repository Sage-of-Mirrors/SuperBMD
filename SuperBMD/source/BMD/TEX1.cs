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

            DumpTextures("D:\\SuperBMD\\TexTest\\gnd_textest");
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

        public byte[] ToBytes()
        {
            List<byte> outList = new List<byte>();

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(mem, Endian.Big);

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

                // Palette pass
                for (int i = 0; i < imgData.Count; i++)
                {
                    writer.Seek((i * 32) + 44, System.IO.SeekOrigin.Begin); // offset of the image header + 32 bytes for the section's header + 12 bytes into the image header for palette offset
                    writer.Write((int)writer.BaseStream.Length - (32 + i * 32)); // Offsets are relative to the start of the image header
                    writer.Seek(0, System.IO.SeekOrigin.End);

                    if (imgData[i].Item2.Length > 0)
                    {
                        foreach (ushort st in imgData[i].Item2)
                            writer.Write(st);

                        StreamUtility.PadStreamWithString(writer, 32);
                    }
                }

                // Image data pass
                for (int i = 0; i < imgData.Count; i++)
                {
                    writer.Seek((i * 32) + 60, System.IO.SeekOrigin.Begin); // offset of the image header + 32 bytes for the section's header + 28 bytes into the image header for image data offset
                    writer.Write((int)writer.BaseStream.Length - (32 + i * 32)); // Offsets are relative to the start of the image header
                    writer.Seek(0, System.IO.SeekOrigin.End);

                    writer.Write(imgData[i].Item1);
                }

                writer.Seek(16, System.IO.SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length);
                writer.Seek(0, System.IO.SeekOrigin.End);

                writer.Write(NameTableIO.Write(names));

                StreamUtility.PadStreamWithString(writer, 32);

                writer.Seek(4, System.IO.SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length);

                outList.AddRange(mem.ToArray());
            }

            return outList.ToArray();
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
