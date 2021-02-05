using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SuperBMD.Util
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Convert a Quaternion to Euler Angles. Returns the angles in [-180, 180] space in degrees.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quat"></param>
        /// <returns></returns>
        public static Vector3 ToEulerAngles(this Quaternion quat)
        {
            return new Vector3(WMath.RadiansToDegrees(PitchFromQuat(quat)), WMath.RadiansToDegrees(YawFromQuat(quat)), WMath.RadiansToDegrees(RollFromQuat(quat)));
        }

        /// <summary>
        /// Create a Quaternion from Euler Angles. These should be in degrees in [-180, 180] space.
        /// </summary>
        public static Quaternion FromEulerAngles(this Quaternion quat, Vector3 eulerAngles)
        {
            eulerAngles.X = WMath.DegreesToRadians(eulerAngles.X);
            eulerAngles.Y = WMath.DegreesToRadians(eulerAngles.Y);
            eulerAngles.Z = WMath.DegreesToRadians(eulerAngles.Z);

            double c1 = Math.Cos(eulerAngles.Y / 2f);
            double s1 = Math.Sin(eulerAngles.Y / 2f);
            double c2 = Math.Cos(eulerAngles.X / 2f);
            double s2 = Math.Sin(eulerAngles.X / 2f);
            double c3 = Math.Cos(eulerAngles.Z / 2f);
            double s3 = Math.Sin(eulerAngles.Z / 2f);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;

            float w = (float)(c1c2 * c3 - s1s2 * s3);
            float x = (float)(c1c2 * s3 + s1s2 * c3);
            float y = (float)(s1 * c2 * c3 + c1 * s2 * s3);
            float z = (float)(c1 * s2 * c3 - s1 * c2 * s3);
            return new Quaternion(x, y, z, w);
        }

        private static float PitchFromQuat(Quaternion q)
        {
            return (float)Math.Atan2(2f * (q.W * q.X + q.Y * q.Z), 1 - (2 * (Math.Pow(q.X, 2) + Math.Pow(q.Y, 2))));
        }

        private static float YawFromQuat(Quaternion q)
        {
            return (float)Math.Asin(2f * (q.W * q.Y - q.X * q.Z));
        }

        private static float RollFromQuat(Quaternion q)
        {
            return (float)Math.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - (2 * (Math.Pow(q.Y, 2) + Math.Pow(q.Z, 2))));
        }
    }

    public static class WMath
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return (1 - t) * a + t * b;
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180.0);
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * (float)(180.0 / Math.PI);
        }

        public static float RotationShortToFloat(short rotation)
        {
            return rotation * (180 / 32768f);
        }

        public static short RotationFloatToShort(float rotation)
        {
            return (short)(rotation * (32768f / 180f));
        }

        /// <summary>
        /// Calculate the number of bytes required to pad the specified
        /// number up to the next 16 byte alignment.
        /// </summary>
        /// <param name="inPos">Position in memory stream that you're currently at.</param>
        /// <returns>The delta required to get to the next 16 byte alignment.</returns>
        public static int Pad16Delta(long inPos)
        {
            // Pad up to a 16 byte alignment
            // Formula: (x + (n-1)) & ~(n-1)
            long nextAligned = (inPos + 0xF) & ~0xF;

            long delta = nextAligned - inPos;
            return (int)delta;
        }

        /// <summary>
        /// Calculate the number of bytes required to pad the specified
        /// number up to the next 32 byte alignment.
        /// </summary>
        /// <param name="inPos">Position in memory stream that you're currently at.</param>
        /// <returns>The delta required to get to the next 32 byte alignment.</returns>
        public static int Pad32Delta(long inPos)
        {
            // Pad up to a 32 byte alignment
            // Formula: (x + (n-1)) & ~(n-1)
            long nextAligned = (inPos + 0x1F) & ~0x1F;

            long delta = nextAligned - inPos;
            return (int)delta;
        }

        public static int Floor(float val)
        {
            return (int)Math.Floor(val);
        }
    }
}
