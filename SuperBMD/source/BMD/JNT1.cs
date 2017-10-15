using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Rigging;
using OpenTK;
using GameFormatReader.Common;
using SuperBMD.Util;
using Assimp;

namespace SuperBMD.BMD
{
    public class JNT1
    {
        List<Rigging.Bone> FlatSkeleton;
        Rigging.Bone SkeletonRoot;

        public JNT1(EndianBinaryReader reader, int offset)
        {
            FlatSkeleton = new List<Rigging.Bone>();

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
            reader.SkipInt32();

            int jnt1Size = reader.ReadInt32();
            int jointCount = reader.ReadInt16();
            reader.SkipInt16();
            int jointDataOffset = reader.ReadInt32();
            int internTableOffset = reader.ReadInt32();
            int nameTableOffset = reader.ReadInt32();

            reader.BaseStream.Seek(nameTableOffset + offset, System.IO.SeekOrigin.Begin);
            List<string> names = NameTableIO.Load(reader, nameTableOffset);

            int highestRemap = 0;
            List<int> remapTable = new List<int>();
            reader.BaseStream.Seek(offset + internTableOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < jointCount; i++)
            {
                int test = reader.ReadInt16();
                remapTable.Add(test);

                if (test > highestRemap)
                    highestRemap = test;
            }

            List<Rigging.Bone> tempList = new List<Rigging.Bone>();
            reader.BaseStream.Seek(offset + jointDataOffset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i <= highestRemap; i++)
            {
                tempList.Add(new Rigging.Bone(reader, names[i]));
            }

            for (int i = 0; i < jointCount; i++)
            {
                FlatSkeleton.Add(tempList[remapTable[i]]);
            }
        }

        public JNT1(Assimp.Scene scene)
        {
            List<string> boneNames = new List<string>();

            for (int i = 0; i < scene.MeshCount; i++)
            {
                for (int j = 0; j < scene.Meshes[i].BoneCount; j++)
                {
                    if (!boneNames.Contains(scene.Meshes[i].Bones[j].Name))
                        boneNames.Add(scene.Meshes[i].Bones[j].Name);
                }
            }

            List<Assimp.Node> masterNodeList = new List<Node>();
            GetAssimpNodesRecursive(scene.RootNode, masterNodeList);

            List<Assimp.Node> testlist = new List<Node>();

            for (int i = 0; i < masterNodeList.Count; i++)
            {
                if (boneNames.Contains(masterNodeList[i].Name))
                {
                    if (!testlist.Contains(masterNodeList[i]))
                        testlist.Add(masterNodeList[i]);
                }
            }

            List<Assimp.Node> testList2 = new List<Node>();

            foreach (Assimp.Node node in testlist)
            {
                GetNodesRecursive(node, testList2);
            }

            Assimp.Node root;

            foreach (Assimp.Node node in testList2)
            {
                for (int i = 0; i < node.ChildCount; i++)
                {
                    if (!testList2.Contains(node.Children[i]))
                    {
                        for (int j = 0; j < node.ChildCount; j++)
                        {
                            if (testList2.Contains(node.Children[j]))
                            {
                                root = node.Children[j];
                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }

        private void GetAssimpNodesRecursive(Assimp.Node root, List<Assimp.Node> nodes)
        {
            nodes.Add(root);

            for (int i = 0; i < root.ChildCount; i++)
                GetAssimpNodesRecursive(root.Children[i], nodes);
        }

        private void GetNodesRecursive(Assimp.Node node, List<Assimp.Node> list)
        {
            if (node.Parent == null)
                return;

            list.Add(node);

            if (node.Parent != null && !list.Contains(node.Parent))
                GetNodesRecursive(node.Parent, list);
        }
    }
}
