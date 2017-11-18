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
            Color = new Color((float)reader.ReadByte() / 255, (float)reader.ReadByte() / 255, (float)reader.ReadByte() / 255, (float)reader.ReadByte() / 255);

            for (int i = 0; i < 10; i++)
            {
                ushort inVal = reader.ReadUInt16();
                RangeAdjustmentTable[i] = (float)inVal / 256;
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(Enable);
            writer.Write(Center);
            writer.Write(StartZ);
            writer.Write(EndZ);
            writer.Write(NearZ);
            writer.Write(FarZ);
            writer.Write(Color);

            for (int i = 0; i < 10; i++)
                writer.Write((ushort)(RangeAdjustmentTable[i] * 256));
        }
    }
}
