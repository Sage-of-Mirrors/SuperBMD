using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GameFormatReader.Common;

namespace SuperBMD.Rigging
{
    public class Bone
    {
        public string Name { get; private set; }
        public Bone Parent { get; private set; }
        public Matrix4 InverseBindMatrix { get; private set; }
        public Matrix4 TransformationMatrix { get; private set; }

        private short m_MatrixType;
        private byte m_UnknownIndex;
        private Vector3 m_Scale;
        private Quaternion m_Rotation;
        private Vector3 m_Translation;

        private float m_BoundsRadius;
        private Vector3 m_MinimumBounds;
        private Vector3 m_MaximumBounds;

        public Bone(EndianBinaryReader reader, string name)
        {
            Name = name;
            m_MatrixType = reader.ReadInt16();
            m_UnknownIndex = reader.ReadByte();

            reader.SkipByte();

            m_Scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            short xRot = reader.ReadInt16();
            short yRot = reader.ReadInt16();
            short zRot = reader.ReadInt16();
            Vector3 euler = new Vector3(xRot * (180.0f / 32768.0f), yRot * (180.0f / 32768.0f), zRot * (180.0f / 32768.0f));
            m_Rotation = new Quaternion(euler);

            reader.SkipInt16();

            m_Translation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            m_BoundsRadius = reader.ReadSingle();
            m_MinimumBounds = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            m_MaximumBounds = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
