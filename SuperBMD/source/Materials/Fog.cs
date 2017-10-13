using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Util;
using GameFormatReader.Common;

namespace SuperBMD.Materials
{
    public class Fog
    {
        public byte Type;
        public bool Enable;
        public ushort Center;
        public float StartZ;
        public float EndZ;
        public float NearZ;
        public float FarZ;
        public Color Color;
        public float[] RangeAdjustmentTable;

        public Fog(EndianBinaryReader reader)
        {
            RangeAdjustmentTable = new float[10];

            Type = reader.ReadByte();
            Enable = reader.ReadBoolean();
            Center = reader.ReadUInt16();
            StartZ = reader.ReadSingle();
            EndZ = reader.ReadSingle();
            NearZ = reader.ReadSingle();
            FarZ = reader.ReadSingle();
            Color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

            for (int i = 0; i < 10; i++)
            {
                ushort inVal = reader.ReadUInt16();
                RangeAdjustmentTable[i] = (float)inVal / 256;
            }
        }
    }
}
