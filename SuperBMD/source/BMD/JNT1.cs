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
            FlatSkeleton = new List<Rigging.Bone>();
            Assimp.Node root = null;

            for (int i = 0; i < scene.RootNode.ChildCount; i++)
            {
                if (scene.RootNode.Children[i].Name.ToLowerInvariant() == "skeleton_root")
                {
                    root = scene.RootNode.Children[i].Children[0];
                    break;
                }
            }

            if (root == null)
                throw new Exception("Skeleton root was not found. Please make sure the root is under a node called \"skeleton_root.\"");

            SkeletonRoot = AssimpNodesToBonesRecursive(root, null, FlatSkeleton);
        }

        private Rigging.Bone AssimpNodesToBonesRecursive(Assimp.Node node, Rigging.Bone parent, List<Rigging.Bone> boneList)
        {
            Rigging.Bone newBone = new Rigging.Bone(node, parent);
            boneList.Add(newBone);

            for (int i = 0; i < node.ChildCount; i++)
            {
                newBone.Children.Add(AssimpNodesToBonesRecursive(node.Children[i], newBone, boneList));
            }

            return newBone;
        }
    }
}
