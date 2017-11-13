using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperBMD.Materials.Enums;
using GameFormatReader.Common;
using OpenTK;

namespace SuperBMD.Util
{
    public static class J3DUtility
    {
        public static GXColorChannelId ToGXColorChannelId(this J3DColorChannelId id)
        {
            switch (id)
            {
                case J3DColorChannelId.Color0: return GXColorChannelId.Color0;
                case J3DColorChannelId.Color1: return GXColorChannelId.Color1;
                case J3DColorChannelId.Alpha0: return GXColorChannelId.Alpha0;
                case J3DColorChannelId.Alpha1: return GXColorChannelId.Alpha1;
            }

            throw new ArgumentOutOfRangeException("id");
        }

        public static J3DColorChannelId ToJ3DColorChannelId(this GXColorChannelId id)
        {
            switch (id)
            {
                case GXColorChannelId.Color0: return J3DColorChannelId.Color0;
                case GXColorChannelId.Color1: return J3DColorChannelId.Color1;
                case GXColorChannelId.Alpha0: return J3DColorChannelId.Alpha0;
                case GXColorChannelId.Alpha1: return J3DColorChannelId.Alpha1;
            }

            throw new ArgumentOutOfRangeException("id");
        }

        public static GXAttenuationFn ToGXAttenuationFn(this J3DAttenuationFn fn)
        {
            switch (fn)
            {
                case J3DAttenuationFn.None_0:
                case J3DAttenuationFn.None_2: return GXAttenuationFn.None;
                case J3DAttenuationFn.Spec:   return GXAttenuationFn.Spec;
                case J3DAttenuationFn.Spot:   return GXAttenuationFn.Spot;
            }

            throw new ArgumentOutOfRangeException("fn");
        }

        public static J3DAttenuationFn ToJ3DAttenuationFn(this GXAttenuationFn fn)
        {
            switch (fn)
            {
                case GXAttenuationFn.None: return J3DAttenuationFn.None_0;
                case GXAttenuationFn.Spec: return J3DAttenuationFn.Spec;
                case GXAttenuationFn.Spot: return J3DAttenuationFn.Spot;
            }

            throw new ArgumentOutOfRangeException("fn");
        }

        public static int GetHighestValue(List<int> input)
        {
            int highest = int.MinValue;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] > highest)
                    highest = input[i];
            }

            return highest;
        }

        public static ushort[] CompressRotation(Vector3 rot)
        {
            ushort[] output = new ushort[3];

            output[0] = (ushort)((rot.X * 32767.0f) / 180.0f);
            output[1] = (ushort)((rot.Y * 32767.0f) / 180.0f);
            output[2] = (ushort)((rot.Z * 32767.0f) / 180.0f);

            return output;
        }

        public static Vector3 ToEulerAngles(this Quaternion q)
        {
            // Store the Euler angles in radians
            Vector3 pitchYawRoll = new Vector3();

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.4999f * unit)                              // 0.4999f OR 0.5f - EPSILON
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitchYawRoll.X = (float)Math.PI * 0.5f;                         // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.4999f * unit)                        // -0.4999f OR -0.5f + EPSILON
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitchYawRoll.X = -(float)Math.PI * 0.5f;                        // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else
            {
                pitchYawRoll.Y = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
                pitchYawRoll.X = (float)Math.Asin(2f * test / unit);                                             // Pitch
                pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll
            }

            pitchYawRoll.X = pitchYawRoll.X * (float)(180 / Math.PI);
            pitchYawRoll.Y = pitchYawRoll.Y * (float)(180 / Math.PI);
            pitchYawRoll.Z = pitchYawRoll.Z * (float)(180 / Math.PI);

            return pitchYawRoll;

            /*
            Vector3 vec = new Vector3();

            float ysqr = q.Y * q.Y;

            float t0 = 2.0f * (q.W * q.X + q.Y * q.Z);
            float t1 = 1.0f - 2.0f * (q.X * q.X + ysqr);

            vec.X = (float)Math.Atan2(t0, t1);

            float t2 = 2.0f * (q.W * q.Y - q.Z * q.X);
            t2 = t2 > 1.0f ? 1.0f : t2;
            t2 = t2 < -1.0f ? -1.0f : t2;

            vec.Y = (float)Math.Asin(t2);

            float t3 = 2.0f * (q.W * q.Z + q.X * q.Y);
            float t4 = 1.0f - 2.0f * (ysqr + q.Z * q.Z);

            vec.Z = (float)Math.Atan2(t3, t4);

            vec.X = vec.X * (float)(180.0f / Math.PI);
            vec.Y = vec.Y * (float)(180.0f / Math.PI);
            vec.Z = vec.Z * (float)(180.0f / Math.PI);

            return vec;*/
        }
    }
}
