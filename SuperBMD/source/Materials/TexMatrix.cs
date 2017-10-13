using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GameFormatReader.Common;
using SuperBMD.Materials.Enums;

namespace SuperBMD.Materials
{
    public class TexMatrix
    {
        public TexGenType Projection;
        public byte Type;
        public Vector3 EffectTranslation;

        public Vector2 Scale;
        public float Rotation;
        public Vector2 Translation;

        public Matrix4 ProjectionMatrix;

        public TexMatrix(EndianBinaryReader reader)
        {
            Projection = (TexGenType)reader.ReadByte();
            Type = reader.ReadByte();
            reader.SkipInt16();
            EffectTranslation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Rotation = reader.ReadInt16() * (180 / 32768f);
            Translation = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            ProjectionMatrix = new Matrix4(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
