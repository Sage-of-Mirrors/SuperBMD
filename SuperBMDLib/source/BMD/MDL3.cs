using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMDLib.Materials;
using SuperBMDLib.Materials.Mdl;
using SuperBMDLib.Util;
using GameFormatReader.Common;

namespace SuperBMDLib.BMD
{
    public class MDL3
    {
        List<MdlEntry> Entries;

        public MDL3()
        {
            Entries = new List<MdlEntry>();
        }

        public MDL3(List<Material> materials, List<BinaryTextureImage> textures)
        {
            Entries = new List<MdlEntry>();

            foreach (Material mat in materials)
                Entries.Add(new MdlEntry(mat, textures));
        }

        public void Write(EndianBinaryWriter writer)
        {
            long start = writer.BaseStream.Position;

            writer.Write("MDL3".ToCharArray());
            writer.Write(0); // Placeholder for section size
            writer.Write((short)Entries.Count);
            writer.Write((short)-1);

            writer.Write(0x40); // Offset to command data offset/size block
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            StreamUtility.PadStreamWithString(writer, 32);

            long cmdBlockStart = writer.BaseStream.Position;

            for (int i = 0; i < Entries.Count; i ++)
            {
                writer.Write(0);
                writer.Write(0);
            }

            StreamUtility.PadStreamWithString(writer, 32);

            for (int i = 0; i < Entries.Count; i++)
            {
                long absoluteStartOffset = writer.BaseStream.Position;
                long relativeStartOffset = writer.BaseStream.Position - cmdBlockStart - i * 8;

                Entries[i].Write(writer);

                long size = writer.BaseStream.Position - absoluteStartOffset;
                writer.Seek((int)cmdBlockStart + (i * 8), System.IO.SeekOrigin.Begin);

                writer.Write((int)relativeStartOffset);
                writer.Write((int)size);

                writer.Seek(0, System.IO.SeekOrigin.End);
            }

            StreamUtility.PadStreamWithString(writer, 32);

            long end = writer.BaseStream.Position;
            long length = (end - start);

            writer.Seek((int)start + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)length);
            writer.Seek((int)end, System.IO.SeekOrigin.Begin);
        }
    }
}
