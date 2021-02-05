using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBMDLib.Animation
{
    public enum TangentMode
    {
        Symmetric,
        Piecewise
    }

    public struct Keyframe
    {
        public float Time;
        public float InTangent;
        public float OutTangent;
        public float Key;
    }

    public struct Track
    {
        public Keyframe[][] Translation;
        public Keyframe[][] Rotation;
        public Keyframe[][] Scale;

        public bool IsIdentity;

        public static Track Identity()
        {
            Track ident_track = new Track();
            ident_track.IsIdentity = true;

            ident_track.Translation = new Keyframe[][]
            {
                new Keyframe[] {},
                new Keyframe[] {},
                new Keyframe[] {},
            };

            ident_track.Rotation = new Keyframe[][]
            {
                new Keyframe[] {},
                new Keyframe[] {},
                new Keyframe[] {},
            };

            ident_track.Scale = new Keyframe[][]
            {
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = 1, OutTangent = 0, Time = 0} },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = 1, OutTangent = 0, Time = 0} },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = 1, OutTangent = 0, Time = 0} },
            };

            return ident_track;
        }
    }
}
