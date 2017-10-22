using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Scenegraph.Enums;
using GameFormatReader.Common;

namespace SuperBMD.Scenegraph
{
    public class SceneNode
    {
        public SceneNode Parent { get; private set; }
        public List<SceneNode> Children { get; private set; }

        public NodeType Type { get; private set; }
        public int Index { get; private set; }

        public SceneNode(EndianBinaryReader reader, SceneNode parent)
        {
            Children = new List<SceneNode>();
            Parent = parent;

            if (parent != null)
                parent.Children.Add(this);

            Type = (NodeType)reader.ReadInt16();
            Index = reader.ReadInt16();
        }

        public override string ToString()
        {
            return $"{ Type } : { Index }";
        }
    }
}
