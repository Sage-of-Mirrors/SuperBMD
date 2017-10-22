using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using SuperBMD.Geometry.Enums;

namespace SuperBMD.Geometry
{
    public class ShapeVertexDescriptor
    {
        public SortedDictionary<GXVertexAttribute, Tuple<VertexInputType, int>> Attributes { get; private set; }

        public ShapeVertexDescriptor()
        {
            Attributes = new SortedDictionary<GXVertexAttribute, Tuple<VertexInputType, int>>();
        }

        public ShapeVertexDescriptor(EndianBinaryReader reader, int offset)
        {
            Attributes = new SortedDictionary<GXVertexAttribute, Tuple<VertexInputType, int>>();
            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            int index = 0;
            GXVertexAttribute attrib = (GXVertexAttribute)reader.ReadInt32();

            while (attrib != GXVertexAttribute.Null)
            {
                Attributes.Add(attrib, new Tuple<VertexInputType, int>((VertexInputType)reader.ReadInt32(), index));

                index++;
                attrib = (GXVertexAttribute)reader.ReadInt32();
            }
        }

        public bool CheckAttribute(GXVertexAttribute attribute)
        {
            return Attributes.ContainsKey(attribute);
        }

        public void SetAttribute(GXVertexAttribute attribute, VertexInputType inputType, int vertexIndex)
        {
            if (CheckAttribute(attribute))
                throw new Exception($"Attribute \"{ attribute }\" is already in the vertex descriptor!");

            Attributes.Add(attribute, new Tuple<VertexInputType, int>(inputType, vertexIndex));
        }

        public List<GXVertexAttribute> GetActiveAttributes()
        {
            List<GXVertexAttribute> attribs = new List<GXVertexAttribute>(Attributes.Keys);
            return attribs;
        }

        public int GetAttributeIndex(GXVertexAttribute attribute)
        {
            if (CheckAttribute(attribute))
                return Attributes[attribute].Item2;
            else
                throw new ArgumentException("attribute");
        }

        public VertexInputType GetAttributeType(GXVertexAttribute attribute)
        {
            if (CheckAttribute(attribute))
                return Attributes[attribute].Item1;
            else
                throw new ArgumentException("attribute");
        }

        public int CalculateStride()
        {
            int stride = 0;

            foreach (Tuple<VertexInputType, int> tup in Attributes.Values)
            {
                switch(tup.Item1)
                {
                    case VertexInputType.Index16:
                        stride += 2;
                        break;
                    case VertexInputType.Index8:
                    case VertexInputType.Direct: // HACK: BMD usually uses this only for PositionMatrixIdx, which uses a byte, but we should really use the VAT/VTX1 to get the actual stride
                        stride += 1;
                        break;
                    case VertexInputType.None:
                        break;
                    default:
                        throw new Exception($"Unknown vertex input type\"{ tup.Item1 }\"");
                }
            }

            return stride;
        }
    }
}
