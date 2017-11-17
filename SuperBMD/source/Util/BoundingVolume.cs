using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using GameFormatReader.Common;

namespace SuperBMD.Util
{
    public class BoundingVolume
    {
        public float SphereRadius { get; private set; }
        public Vector3 MinBounds { get; private set; }
        public Vector3 MaxBounds { get; private set; }

        public BoundingVolume()
        {
            MinBounds = new Vector3();
            MaxBounds = new Vector3();
        }

        public BoundingVolume(EndianBinaryReader reader)
        {
            SphereRadius = reader.ReadSingle();

            MinBounds = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            MaxBounds = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(SphereRadius);
            writer.Write(MinBounds);
            writer.Write(MaxBounds);
        }
    }
}
