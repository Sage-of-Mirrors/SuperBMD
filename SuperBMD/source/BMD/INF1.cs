using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Scenegraph;
using SuperBMD.Scenegraph.Enums;
using GameFormatReader.Common;

namespace SuperBMD.BMD
{
    public class INF1
    {
        public List<SceneNode> FlatNodes { get; private set; }
        public SceneNode Root { get; private set; }

        public INF1(EndianBinaryReader reader, int offset)
        {
            FlatNodes = new List<SceneNode>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();
            int inf1Size = reader.ReadInt32();
            int unk1 = reader.ReadInt16();
            reader.SkipInt16();

            int packetCount = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            int hierarchyOffset = reader.ReadInt32();

            SceneNode parent = new SceneNode(reader, null);
            SceneNode node = null;

            Root = parent;
            FlatNodes.Add(parent);

            do
            {
                node = new SceneNode(reader, parent);

                FlatNodes.Add(node);

                if (node.Type == NodeType.OpenChild)
                {
                    SceneNode newNode = new SceneNode(reader, node.Parent);
                    FlatNodes.Add(newNode);
                    parent = newNode;
                }
                else if (node.Type == NodeType.CloseChild)
                    parent = node.Parent;

            } while (node.Type != NodeType.Terminator);

            reader.BaseStream.Seek(offset + inf1Size, System.IO.SeekOrigin.Begin);
        }
    }
}
