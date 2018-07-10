using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMDLib.Geometry.Enums;
using SuperBMDLib.Rigging;
using GameFormatReader.Common;

namespace SuperBMDLib.Geometry
{
    public class Vertex
    {
        public uint PositionMatrixIDxIndex { get; private set; }
        public uint PositionIndex { get; private set; }
        public uint NormalIndex { get; private set; }
        public uint Color0Index { get; private set; }
        public uint Color1Index { get; private set; }
        public uint TexCoord0Index { get; private set; }
        public uint TexCoord1Index { get; private set; }
        public uint TexCoord2Index { get; private set; }
        public uint TexCoord3Index { get; private set; }
        public uint TexCoord4Index { get; private set; }
        public uint TexCoord5Index { get; private set; }
        public uint TexCoord6Index { get; private set; }
        public uint TexCoord7Index { get; private set; }

        public uint PositionMatrixIndex { get; set; }
        public uint NormalMatrixIndex { get; set; }

        public Weight VertexWeight { get; private set; }

        public Vertex()
        {
            PositionMatrixIDxIndex = uint.MaxValue;
            PositionIndex = uint.MaxValue;
            NormalIndex = uint.MaxValue;
            Color0Index = uint.MaxValue;
            Color1Index = uint.MaxValue;
            TexCoord0Index = uint.MaxValue;
            TexCoord1Index = uint.MaxValue;
            TexCoord2Index = uint.MaxValue;
            TexCoord3Index = uint.MaxValue;
            TexCoord4Index = uint.MaxValue;
            TexCoord5Index = uint.MaxValue;
            TexCoord6Index = uint.MaxValue;
            TexCoord7Index = uint.MaxValue;

            VertexWeight = new Weight();
        }

        public uint GetAttributeIndex(GXVertexAttribute attribute)
        {
            switch (attribute)
            {
                case GXVertexAttribute.PositionMatrixIdx:
                    return PositionMatrixIDxIndex;
                case GXVertexAttribute.Position:
                    return PositionIndex;
                case GXVertexAttribute.Normal:
                    return NormalIndex;
                case GXVertexAttribute.Color0:
                    return Color0Index;
                case GXVertexAttribute.Color1:
                    return Color1Index;
                case GXVertexAttribute.Tex0:
                    return TexCoord0Index;
                case GXVertexAttribute.Tex1:
                    return TexCoord1Index;
                case GXVertexAttribute.Tex2:
                    return TexCoord2Index;
                case GXVertexAttribute.Tex3:
                    return TexCoord3Index;
                case GXVertexAttribute.Tex4:
                    return TexCoord4Index;
                case GXVertexAttribute.Tex5:
                    return TexCoord5Index;
                case GXVertexAttribute.Tex6:
                    return TexCoord6Index;
                case GXVertexAttribute.Tex7:
                    return TexCoord7Index;
                default:
                    throw new ArgumentException("attribute");
            }
        }

        public void SetAttributeIndex(GXVertexAttribute attribute, uint index)
        {
            switch (attribute)
            {
                case GXVertexAttribute.PositionMatrixIdx:
                    PositionMatrixIDxIndex = index;
                    break;
                case GXVertexAttribute.Position:
                    PositionIndex = index;
                    break;
                case GXVertexAttribute.Normal:
                    NormalIndex = index;
                    break;
                case GXVertexAttribute.Color0:
                    Color0Index = index;
                    break;
                case GXVertexAttribute.Color1:
                    Color1Index = index;
                    break;
                case GXVertexAttribute.Tex0:
                    TexCoord0Index = index;
                    break;
                case GXVertexAttribute.Tex1:
                    TexCoord1Index = index;
                    break;
                case GXVertexAttribute.Tex2:
                    TexCoord2Index = index;
                    break;
                case GXVertexAttribute.Tex3:
                    TexCoord3Index = index;
                    break;
                case GXVertexAttribute.Tex4:
                    TexCoord4Index = index;
                    break;
                case GXVertexAttribute.Tex5:
                    TexCoord5Index = index;
                    break;
                case GXVertexAttribute.Tex6:
                    TexCoord6Index = index;
                    break;
                case GXVertexAttribute.Tex7:
                    TexCoord7Index = index;
                    break;
                default:
                    throw new ArgumentException("attribute");
            }
        }

        public void SetWeight(Weight weight)
        {
            VertexWeight = weight;
        }

        public void Write(EndianBinaryWriter writer, ShapeVertexDescriptor desc)
        {
            if (desc.CheckAttribute(GXVertexAttribute.PositionMatrixIdx))
            {
                WriteAttributeIndex(writer, PositionMatrixIDxIndex * 3, desc.Attributes[GXVertexAttribute.PositionMatrixIdx].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Position))
            {
                WriteAttributeIndex(writer, PositionIndex, desc.Attributes[GXVertexAttribute.Position].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Normal))
            {
                WriteAttributeIndex(writer, NormalIndex, desc.Attributes[GXVertexAttribute.Normal].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Color0))
            {
                WriteAttributeIndex(writer, Color0Index, desc.Attributes[GXVertexAttribute.Color0].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Color1))
            {
                WriteAttributeIndex(writer, Color1Index, desc.Attributes[GXVertexAttribute.Color1].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex0))
            {
                WriteAttributeIndex(writer, TexCoord0Index, desc.Attributes[GXVertexAttribute.Tex0].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex1))
            {
                WriteAttributeIndex(writer, TexCoord1Index, desc.Attributes[GXVertexAttribute.Tex1].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex2))
            {
                WriteAttributeIndex(writer, TexCoord2Index, desc.Attributes[GXVertexAttribute.Tex2].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex3))
            {
                WriteAttributeIndex(writer, TexCoord3Index, desc.Attributes[GXVertexAttribute.Tex3].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex4))
            {
                WriteAttributeIndex(writer, TexCoord4Index, desc.Attributes[GXVertexAttribute.Tex4].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex5))
            {
                WriteAttributeIndex(writer, TexCoord5Index, desc.Attributes[GXVertexAttribute.Tex5].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex6))
            {
                WriteAttributeIndex(writer, TexCoord6Index, desc.Attributes[GXVertexAttribute.Tex6].Item1);
            }

            if (desc.CheckAttribute(GXVertexAttribute.Tex7))
            {
                WriteAttributeIndex(writer, TexCoord7Index, desc.Attributes[GXVertexAttribute.Tex7].Item1);
            }
        }

        private void WriteAttributeIndex(EndianBinaryWriter writer, uint value, VertexInputType type)
        {
            switch (type)
            {
                case VertexInputType.Direct:
                case VertexInputType.Index8:
                    writer.Write((byte)value);
                    break;
                case VertexInputType.Index16:
                    writer.Write((short)value);
                    break;
                case VertexInputType.None:
                default:
                    throw new ArgumentException("vertex input type");
            }
        }
    }
}
