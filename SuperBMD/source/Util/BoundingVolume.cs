using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SuperBMD.Util
{
    public class BoundingVolume
    {
        public float SphereRadius { get; private set; }
        public Vector3 MinBounds { get; private set; }
        public Vector3 MaxBounds { get; private set; }
    }
}
