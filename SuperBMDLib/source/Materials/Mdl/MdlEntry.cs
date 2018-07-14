using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using SuperBMDLib.Materials.Enums;

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

        public MdlEntry(Material mat, List<BinaryTextureImage> textures)
        {
            Name = mat.Name;
            BPCommands = new List<BPCommand>();
            XFCommands = new List<XFCommand>();

            GenerateTextureCommands(mat, textures);
            GenerateTexGenCommands(mat);
        }

        private void GenerateTextureCommands(Material mat, List<BinaryTextureImage> textures)
        {
            BPRegister[] texInfoRegs = new BPRegister[] { BPRegister.TX_SETIMAGE0_I0, BPRegister.TX_SETIMAGE0_I1, BPRegister.TX_SETIMAGE0_I2,
                                                          BPRegister.TX_SETIMAGE0_I3, BPRegister.TX_SETIMAGE0_I4, BPRegister.TX_SETIMAGE0_I5,
                                                          BPRegister.TX_SETIMAGE0_I6, BPRegister.TX_SETIMAGE0_I7 };

            BPRegister[] texIndexRegs = new BPRegister[] { BPRegister.TX_SETIMAGE3_I0, BPRegister.TX_SETIMAGE3_I1, BPRegister.TX_SETIMAGE3_I2,
                                                           BPRegister.TX_SETIMAGE3_I3, BPRegister.TX_SETIMAGE3_I4, BPRegister.TX_SETIMAGE3_I5,
                                                           BPRegister.TX_SETIMAGE3_I6, BPRegister.TX_SETIMAGE3_I7 };

            BPRegister[] texMode0Regs = new BPRegister[] { BPRegister.TX_SET_MODE0_I0, BPRegister.TX_SET_MODE0_I1, BPRegister.TX_SET_MODE0_I2,
                                                           BPRegister.TX_SET_MODE0_I3, BPRegister.TX_SET_MODE0_I4, BPRegister.TX_SET_MODE0_I5,
                                                           BPRegister.TX_SET_MODE0_I6, BPRegister.TX_SET_MODE0_I7 };

            BPRegister[] texMode1Regs = new BPRegister[] { BPRegister.TX_SET_MODE1_I0, BPRegister.TX_SET_MODE1_I1, BPRegister.TX_SET_MODE1_I2,
                                                           BPRegister.TX_SET_MODE1_I3, BPRegister.TX_SET_MODE1_I4, BPRegister.TX_SET_MODE1_I5,
                                                           BPRegister.TX_SET_MODE1_I6, BPRegister.TX_SET_MODE1_I7 };

            for (int i = 0; i < 8; i++)
            {
                if (mat.TextureIndices[i] == -1)
                    continue;

                BinaryTextureImage curTex = textures[mat.TextureIndices[i]];

                // Records width, height, and format
                BPCommand texInfo = new BPCommand() { Register = texInfoRegs[i] };
                texInfo.SetBits(curTex.Width - 1, 0, 10);
                texInfo.SetBits(curTex.Height - 1, 10, 10);
                texInfo.SetBits((int)curTex.Format, 20, 4);

                // Records the index of the texture
                BPCommand texIndex = new BPCommand() { Register = texIndexRegs[i] };
                texIndex.SetBits(mat.TextureIndices[i], 0, 24);

                BPCommand mode0 = new BPCommand() { Register = texMode0Regs[i] };
                mode0.SetBits((int)curTex.WrapS, 0, 2);
                mode0.SetBits((int)curTex.WrapT, 2, 2);
                mode0.SetBits((int)curTex.MagFilter, 4, 1);
                mode0.SetBits((int)curTex.MinFilter, 5, 3);
                mode0.SetFlag(true, 8); // Edge LOD
                // LOADBias not supported in bmd2bdl
                mode0.SetBits(0, 19, 2); // Max aniso
                mode0.SetFlag(true, 21); // Bias Clamp
                // MinLOD
                // MaxLOD

                BPCommand mode1 = new BPCommand() { Register = texMode1Regs[i] };

                BPCommands.Add(texIndex);
                BPCommands.Add(texInfo);
                BPCommands.Add(mode0);
                BPCommands.Add(mode1);
            }
        }

        private void GenerateTexGenCommands(Material mat)
        {
            for (int i = 0; i < 8; i++)
            {
                if (mat.TexCoord1Gens[i] == null)
                    continue;

                BPCommand suSizeMask = new BPCommand() { Register = BPRegister.SS_MASK };
                suSizeMask.SetBits(0x03FFFF, 0, 24);

                BPCommand sSize = new BPCommand() { Register = BPRegister.SU_SSIZE0 + (i * 2) };
                sSize.SetFlag(false, 16);
                BPCommand tSize = new BPCommand() { Register = BPRegister.SU_TSIZE0 + (i * 2) };
                sSize.SetFlag(false, 16);

                BPCommands.Add(suSizeMask);
                BPCommands.Add(sSize);
                BPCommands.Add(tSize);
            }
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
