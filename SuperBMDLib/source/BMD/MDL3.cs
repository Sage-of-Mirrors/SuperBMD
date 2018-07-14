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

        public MDL3(List<Material> materials)
        {
            Entries = new List<MdlEntry>();

            foreach (Material mat in materials)
                Entries.Add(new MdlEntry(mat));
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
                long startOffset = writer.BaseStream.Position - cmdBlockStart;

                Entries[i].Write(writer);

                long size = writer.BaseStream.Position - startOffset;
                writer.Seek((int)cmdBlockStart + (i * 8), System.IO.SeekOrigin.Begin);

                writer.Write((int)startOffset);
                writer.Write((int)size);

                writer.Seek(0, System.IO.SeekOrigin.End);
            }
        }
    }
}
