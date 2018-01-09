using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Scenegraph;
using SuperBMD.Scenegraph.Enums;
using GameFormatReader.Common;
using Assimp;
using SuperBMD.Util;

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
                SceneNode matNode = new SceneNode(NodeType.Material, i, Root);
                SceneNode downNode2 = new SceneNode(NodeType.OpenChild, 0, Root);
                SceneNode shapeNode = new SceneNode(NodeType.Shape, i, Root);

                FlatNodes.Add(downNode1);
                FlatNodes.Add(matNode);
                FlatNodes.Add(downNode2);
                FlatNodes.Add(shapeNode);
            }

            if (skeleton.FlatSkeleton.Count > 1)
            {
                foreach (Rigging.Bone bone in skeleton.SkeletonRoot.Children)
                {
                    SceneNode rootChildDown = new SceneNode(NodeType.OpenChild, 0, Root);
                    FlatNodes.Add(rootChildDown);

                    GetNodesRecursive(bone, skeleton.FlatSkeleton, Root);

                    SceneNode rootChildUp = new SceneNode(NodeType.CloseChild, 0, Root);
                    FlatNodes.Add(rootChildUp);
                }
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

        public void FillScene(Scene scene, List<Rigging.Bone> flatSkeleton, bool useSkeletonRoot)
        {
            Node root = scene.RootNode;

            if (useSkeletonRoot)
                root = new Node("skeleton_root");

            ProcessNodesRecursive(root, Root, flatSkeleton);

            if (useSkeletonRoot)
                scene.RootNode.Children.Add(root);
        }

        private void ProcessNodesRecursive(Node rootAssNode, SceneNode rootSceneNode, List<Rigging.Bone> flatSkeleton)
        {
            foreach (SceneNode sceneNode in rootSceneNode.Children)
            {
                if (sceneNode.Type == NodeType.Joint)
                {
                    Rigging.Bone joint = flatSkeleton[sceneNode.Index];

                    Node newAssNode = new Node(joint.Name);
                    newAssNode.Transform = joint.TransformationMatrix.ToMatrix4x4();

                    rootAssNode.Children.Add(newAssNode);
                    ProcessNodesRecursive(newAssNode, sceneNode, flatSkeleton);
                }
                else
                    ProcessNodesRecursive(rootAssNode, sceneNode, flatSkeleton);
            }
        }

        public void CorrectMaterialIndices(Scene scene, MAT3 materials)
        {
            foreach (SceneNode node in FlatNodes)
            {
                if (node.Type == NodeType.Shape)
                {
                    if (node.Index < scene.Meshes.Count)
                    {
                        int matIndex = node.Parent.Index;
                        scene.Meshes[node.Index].MaterialIndex = materials.m_RemapIndices[matIndex];
                    }
                }
            }
        }

        public void Write(EndianBinaryWriter writer, int packetCount, int vertexCount)
        {
            long start = writer.BaseStream.Position;

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

            long end = writer.BaseStream.Position;
            long length = (end - start);

            writer.Seek((int)start + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)length);
            writer.Seek((int)end, System.IO.SeekOrigin.Begin);
        }
    }
}
