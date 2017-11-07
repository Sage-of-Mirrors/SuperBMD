using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Scenegraph;
using SuperBMD.Scenegraph.Enums;
using GameFormatReader.Common;
using Assimp;

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

        public INF1(Scene scene, JNT1 skeleton)
        {
            FlatNodes = new List<SceneNode>();
            Root = new SceneNode(NodeType.Joint, 0, null);
            FlatNodes.Add(Root);

            for (int i = 0; i < scene.MeshCount; i++)
            {
                SceneNode downNode1 = new SceneNode(NodeType.OpenChild, 0, Root);
                SceneNode matNode = new SceneNode(NodeType.Material, scene.Meshes[i].MaterialIndex, Root);
                SceneNode downNode2 = new SceneNode(NodeType.OpenChild, 0, Root);
                SceneNode shapeNode = new SceneNode(NodeType.Shape, i, Root);

                FlatNodes.Add(downNode1);
                FlatNodes.Add(matNode);
                FlatNodes.Add(downNode2);
                FlatNodes.Add(shapeNode);
            }

            foreach (Rigging.Bone bone in skeleton.SkeletonRoot.Children)
            {
                SceneNode rootChildDown = new SceneNode(NodeType.OpenChild, 0, Root);
                FlatNodes.Add(rootChildDown);

                GetNodesRecursive(bone, skeleton.FlatSkeleton, Root);

                SceneNode rootChildUp = new SceneNode(NodeType.CloseChild, 0, Root);
                FlatNodes.Add(rootChildUp);
            }

            for (int i = 0; i < scene.MeshCount * 2; i++)
                FlatNodes.Add(new SceneNode(NodeType.CloseChild, 0, Root));

            FlatNodes.Add(new SceneNode(NodeType.Terminator, 0, Root));
        }

        private void GetNodesRecursive(Rigging.Bone bone, List<Rigging.Bone> skeleton, SceneNode parent)
        {
            SceneNode node = new SceneNode(NodeType.Joint, skeleton.IndexOf(bone), parent);
            FlatNodes.Add(node);

            foreach (Rigging.Bone child in bone.Children)
            {
                SceneNode downNode = new SceneNode(NodeType.OpenChild, 0, parent);
                FlatNodes.Add(downNode);

                GetNodesRecursive(child, skeleton, node);

                SceneNode upNode = new SceneNode(NodeType.CloseChild, 0, parent);
                FlatNodes.Add(upNode);
            }
        }

        public byte[] ToBytes(int packetCount, int vertexCount)
        {
            List<byte> outList = new List<byte>();

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(mem, Endian.Big);

                writer.Write("INF1".ToCharArray());
                writer.Write(0); // Placeholder for section size
                writer.Write((short)1);
                writer.Write((short)-1);

                writer.Write(packetCount); // Number of packets
                writer.Write(vertexCount); // Number of vertex positions
                writer.Write(0x18);

                foreach (SceneNode node in FlatNodes)
                {
                    writer.Write((short)node.Type);
                    writer.Write((short)node.Index);
                }

                Util.StreamUtility.PadStreamWithString(writer, 32);
                writer.Seek(4, System.IO.SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length);

                outList.AddRange(mem.ToArray());
            }

            return outList.ToArray();
        }
    }
}
