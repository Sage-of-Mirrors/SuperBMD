using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SuperBMD.Util;

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

        public static Track Identity(Matrix4 Transform, float MaxTime)
        {
            Track ident_track = new Track();
            Quaternion XRot = Quaternion.FromAxisAngle(Vector3.UnitX, (float)(0));

            Vector3 Translation = Transform.ExtractTranslation();

            ident_track.Translation = new Keyframe[][]
            {
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Translation.X, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Translation.X, OutTangent = 0, Time = MaxTime } },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Translation.Y, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Translation.Y, OutTangent = 0, Time = MaxTime } },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Translation.Z, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Translation.Z, OutTangent = 0, Time = MaxTime } },
            };

            Quaternion Rotation = Transform.ExtractRotation();
            Vector3 Rot_Vec = QuaternionExtensions.ToEulerAngles(Rotation);

            ident_track.Rotation = new Keyframe[][]
            {
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Rot_Vec.X, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Rot_Vec.X, OutTangent = 0, Time = MaxTime } },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Rot_Vec.Y, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Rot_Vec.Y, OutTangent = 0, Time = MaxTime } },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Rot_Vec.Z, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Rot_Vec.Z, OutTangent = 0, Time = MaxTime } },
            };

            Vector3 Scale = Transform.ExtractScale();

            ident_track.Scale = new Keyframe[][]
            {
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Scale.X, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Scale.X, OutTangent = 0, Time = MaxTime } },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Scale.Y, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Scale.Y, OutTangent = 0, Time = MaxTime } },
                new Keyframe[] { new Keyframe() { InTangent = 0, Key = Scale.Z, OutTangent = 0, Time = 0}, new Keyframe() { InTangent = 0, Key = Scale.Z, OutTangent = 0, Time = MaxTime } },
            };

            return ident_track;
        }
    }
}
