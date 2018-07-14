using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace SuperBMDLib.Materials.Mdl
{
    public class MdlEntry
    {
        string Name;
        List<BPCommand> BPCommands;
        List<XFCommand> XFCommands;

        public MdlEntry()
        {
            BPCommands = new List<BPCommand>();
            XFCommands = new List<XFCommand>();
        }

        public MdlEntry(Material mat)
        {
            Name = mat.Name;
            BPCommands = new List<BPCommand>();
            XFCommands = new List<XFCommand>();


        }

        public void Write(EndianBinaryWriter writer)
        {
            foreach (BPCommand cmd in BPCommands)
                cmd.Write(writer);
            foreach (XFCommand cmd in XFCommands)
                cmd.Write(writer);
        }
    }
}
