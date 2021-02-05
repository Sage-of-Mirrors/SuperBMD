using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using SuperBMDLib.BMD;
using SuperBMDLib.Rigging;
using OpenTK;
using SuperBMD.Util;
using System.IO;

namespace SuperBMDLib.Animation
{
    public enum LoopMode
    {
        Once,
        OnceReset,
        Loop,
        MirroredOnce,
        MirroredLoop
    }

    public class BCK
    {
        public LoopMode LoopMode;
        public byte RotationFrac;
        public short Duration;

        public Track[] Tracks;

        public BCK(Assimp.Animation src_anim, List<Rigging.Bone> bone_list)
        {
            LoopMode = LoopMode.Loop;
            RotationFrac = 0;
            Duration = (short)(src_anim.DurationInTicks * 30.0f);

            Tracks = new Track[bone_list.Count];

            for (int i = 0; i < bone_list.Count; i++)
            {
                Assimp.NodeAnimationChannel node = src_anim.NodeAnimationChannels.Find(x => x.NodeName == bone_list[i].Name);

                if (node == null)
                    Tracks[i] = Track.Identity();
                else
                    Tracks[i] = GenerateTrack(node, bone_list[i]);
            }
        }

        public BCK(EndianBinaryReader reader)
        {
            string magic = new string(reader.ReadChars(8));

            if (magic != "J3D1bck1")
            {
                throw new Exception("File read was not a BCK!");
            }

            int file_size = reader.ReadInt32();
            int section_count = reader.ReadInt32();

            reader.Skip(16);

            ReadAnk1(reader);
        }

        private Track GenerateTrack(Assimp.NodeAnimationChannel channel, Bone bone)
        {
            Track track = new Track();

            track.Translation = GenerateTranslationTrack(channel.PositionKeys, bone);
            track.Rotation = GenerateRotationTrack(channel.RotationKeys, bone);
            track.Scale = GenerateScaleTrack(channel.ScalingKeys, bone);

            return track;
        }

        private Keyframe[][] GenerateTranslationTrack(List<Assimp.VectorKey> keys, Bone bone)
        {
            Keyframe[] x_track = new Keyframe[keys.Count];
            Keyframe[] y_track = new Keyframe[keys.Count];
            Keyframe[] z_track = new Keyframe[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                Assimp.VectorKey current_key = keys[i];
                Vector3 value = new Vector3(current_key.Value.X, current_key.Value.Y, current_key.Value.Z);

                value = Vector3.TransformPosition(value, bone.TransformationMatrix.Inverted());

                x_track[i].Key = value.X;
                x_track[i].Time = (float)current_key.Time;

                y_track[i].Key = value.Y;
                y_track[i].Time = (float)current_key.Time;

                z_track[i].Key = value.Z;
                z_track[i].Time = (float)current_key.Time;
            }

            return new Keyframe[][] { x_track, y_track, z_track };
        }

        private Keyframe[][] GenerateRotationTrack(List<Assimp.QuaternionKey> keys, Bone bone)
        {
            Keyframe[] x_track = new Keyframe[keys.Count];
            Keyframe[] y_track = new Keyframe[keys.Count];
            Keyframe[] z_track = new Keyframe[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                Assimp.QuaternionKey current_key = keys[i];
                Quaternion value = new Quaternion(current_key.Value.X, current_key.Value.Y, current_key.Value.Z, current_key.Value.W);

                Vector3 quat_as_vec = QuaternionExtensions.ToEulerAngles(value);

                x_track[i].Key = quat_as_vec.X;
                x_track[i].Time = (float)current_key.Time;

                y_track[i].Key = quat_as_vec.Y;
                y_track[i].Time = (float)current_key.Time;

                z_track[i].Key = quat_as_vec.Z;
                z_track[i].Time = (float)current_key.Time;
            }

            return new Keyframe[][] { x_track, y_track, z_track };
        }

        private Keyframe[][] GenerateScaleTrack(List<Assimp.VectorKey> keys, Bone bone)
        {
            Keyframe[] x_track = new Keyframe[keys.Count];
            Keyframe[] y_track = new Keyframe[keys.Count];
            Keyframe[] z_track = new Keyframe[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                Assimp.VectorKey current_key = keys[i];
                Vector3 value = new Vector3(current_key.Value.X, current_key.Value.Y, current_key.Value.Z);

                value *= bone.TransformationMatrix.ExtractScale();

                x_track[i].Key = value.X;
                x_track[i].Time = (float)current_key.Time;

                y_track[i].Key = value.Y;
                y_track[i].Time = (float)current_key.Time;

                z_track[i].Key = value.Z;
                z_track[i].Time = (float)current_key.Time;
            }

            return new Keyframe[][] { x_track, y_track, z_track };
        }

        private void ReadAnk1(EndianBinaryReader reader)
        {
            string magic = new string(reader.ReadChars(4));

            if (magic != "ANK1")
            {
                throw new Exception("Section read was not ANK1!");
            }

            int section_length = reader.ReadInt32();

            LoopMode = (LoopMode)reader.ReadByte();
            RotationFrac = reader.ReadByte();
            Duration = reader.ReadInt16();

            ushort keyframe_count = reader.ReadUInt16();
            ushort scale_count = reader.ReadUInt16();
            ushort rotation_count = reader.ReadUInt16();
            ushort translation_count = reader.ReadUInt16();

            int anim_offset = reader.ReadInt32() + 32;
            int scale_offset = reader.ReadInt32() + 32;
            int rotation_offset = reader.ReadInt32() + 32;
            int translation_offset = reader.ReadInt32() + 32;

            float[] scale_data = ReadFloatTable(scale_offset, scale_count, reader);
            short[] rotation_data = ReadShortTable(rotation_offset, rotation_count, reader);
            float[] translation_data = ReadFloatTable(translation_offset, translation_count, reader);

            Tracks = new Track[keyframe_count];
            reader.BaseStream.Seek(anim_offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < keyframe_count; i++)
            {
                Tracks[i].Translation = new Keyframe[3][];
                Tracks[i].Scale = new Keyframe[3][];
                Tracks[i].Rotation = new Keyframe[3][];

                // X components
                Tracks[i].Scale[0] = ReadFloatKeyframe(reader, scale_data);
                Tracks[i].Rotation[0] = ReadShortKeyframe(reader, rotation_data);
                Tracks[i].Translation[0] = ReadFloatKeyframe(reader, translation_data);

                // Y components
                Tracks[i].Scale[2] = ReadFloatKeyframe(reader, scale_data);
                Tracks[i].Rotation[2] = ReadShortKeyframe(reader, rotation_data);
                Tracks[i].Translation[2] = ReadFloatKeyframe(reader, translation_data);

                // Z components
                Tracks[i].Scale[1] = ReadFloatKeyframe(reader, scale_data);
                Tracks[i].Rotation[1] = ReadShortKeyframe(reader, rotation_data);
                Tracks[i].Translation[1] = ReadFloatKeyframe(reader, translation_data);
            }
        }

        private float[] ReadFloatTable(int offset, int count, EndianBinaryReader reader)
        {
            float[] floats = new float[count];

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < count; i++)
            {
                floats[i] = reader.ReadSingle();
            }

            return floats;
        }

        private short[] ReadShortTable(int offset, int count, EndianBinaryReader reader)
        {
            short[] shorts = new short[count];

            reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < count; i++)
            {
                shorts[i] = reader.ReadInt16();
            }

            return shorts;
        }

        private Keyframe[] ReadFloatKeyframe(EndianBinaryReader reader, float[] data)
        {
            short count = reader.ReadInt16();
            short index = reader.ReadInt16();
            TangentMode tangent_mode = (TangentMode)reader.ReadInt16();

            Keyframe[] key_data = new Keyframe[count];

            for (int i = 0; i < count; i++)
            {
                if (count == 1)
                {
                    key_data[i].Key = data[index];
                    continue;
                }

                int cur_index = index;

                if (tangent_mode == TangentMode.Symmetric)
                {
                    cur_index += 3 * i;
                }
                else
                {
                    cur_index += 4 * i;
                }

                key_data[i].Time = data[cur_index];
                key_data[i].Key = data[cur_index + 1];
                key_data[i].InTangent = data[cur_index + 2];

                if (tangent_mode == TangentMode.Symmetric)
                {
                    key_data[i].OutTangent = key_data[i].InTangent;
                }
                else
                {
                    key_data[i].OutTangent = data[cur_index + 3];
                }
            }

            return key_data;
        }

        private Keyframe[] ReadShortKeyframe(EndianBinaryReader reader, short[] data)
        {
            ushort count = reader.ReadUInt16();
            ushort index = reader.ReadUInt16();
            TangentMode tangent_mode = (TangentMode)reader.ReadInt16();

            Keyframe[] key_data = new Keyframe[count];

            for (int i = 0; i < count; i++)
            {
                if (count == 1)
                {
                    key_data[i].Key = RotationShortToFloat(data[index], RotationFrac);
                    continue;
                }

                int cur_index = index;

                if (tangent_mode == TangentMode.Symmetric)
                {
                    cur_index += 3 * i;
                }
                else
                {
                    cur_index += 4 * i;
                }

                key_data[i].Time = data[cur_index];
                key_data[i].Key = RotationShortToFloat(data[cur_index + 1], RotationFrac);

                key_data[i].InTangent = RotationShortToFloat(data[cur_index + 2], RotationFrac);

                if (tangent_mode == TangentMode.Symmetric)
                {
                    key_data[i].OutTangent = key_data[i].InTangent;
                }
                else
                {
                    key_data[i].OutTangent = RotationShortToFloat(data[cur_index + 3], RotationFrac);
                }
            }

            return key_data;
        }

        public static float RotationShortToFloat(short rot, short rotation_frac)
        {
            float rot_scale = (float)Math.Pow(2f, rotation_frac) * (180f / 32768f);

            //float test = (rot << rotation_frac) * (360.0f / 65536.0F);
            float test = rot * rot_scale;
            return test;
        }

        public static short RotationFloatToShort(float rot, short rotation_frac)
        {
            float rot_scale = (float)Math.Pow(2f, rotation_frac) * (32768f / 180f);

            short test = (short)(rot * rot_scale);
            return test;
        }

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write("J3D1bck1".ToCharArray()); // Magic
            writer.Write((int)0); // Placeholder for filesize
            writer.Write((int)1); // Number of sections. It's only ever 1 for ANK1

            writer.Write("SuperBMD - Gamma".ToCharArray()); // Fill empty space that isn't used

            WriteANK1(writer);

            writer.BaseStream.Seek(8, System.IO.SeekOrigin.Begin);
            writer.Write((int)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }

        private void WriteANK1(EndianBinaryWriter writer)
        {
            long start_offset = writer.BaseStream.Length;

            int ScaleCount;
            int RotCount;
            int TransCount;

            int ScaleOffset;
            int RotOffset;
            int TransOffset;

            byte[] KeyframeData = WriteKeyframedata(out ScaleCount, out RotCount, out TransCount, out ScaleOffset, out RotOffset, out TransOffset);

            writer.Write("ANK1".ToCharArray()); // Magic
            writer.Write((int)0); // Placeholder for section size

            writer.Write((byte)LoopMode);
            writer.Write(RotationFrac);
            writer.Write(Duration);

            writer.Write((short)Tracks.Length);
            writer.Write((short)ScaleCount);
            writer.Write((short)RotCount);
            writer.Write((short)TransCount);

            writer.Write((int)0x40); // Keyframes offset
            writer.Write((int)ScaleOffset); // Scale bank offset
            writer.Write((int)RotOffset); // Rot bank offset
            writer.Write((int)TransOffset); // Trans bank offset

            Util.StreamUtility.PadStreamWithString(writer, 32);

            writer.Write(KeyframeData);

            Util.StreamUtility.PadStreamWithString(writer, 32);

            writer.BaseStream.Seek(start_offset + 4, System.IO.SeekOrigin.Begin);
            writer.Write((int)(writer.BaseStream.Length - start_offset));
            writer.BaseStream.Seek(0, System.IO.SeekOrigin.End);
        }

        private byte[] WriteKeyframedata(out int ScaleCount, out int RotCount, out int TransCount, out int ScaleOffset, out int RotOffset, out int TransOffset)
        {
            List<float> scale_data = new List<float>() { 1.0f };
            List<short> rot_data = new List<short>() { 0 };
            List<float> trans_data = new List<float>() { 0.0f };
            byte[] keyframe_data;

            using (MemoryStream mem = new MemoryStream())
            {
                EndianBinaryWriter keyframe_writer = new EndianBinaryWriter(mem, Endian.Big);

                foreach (Track t in Tracks) // Each bone
                {
                    WriteFloatKeyframe(keyframe_writer, t.Scale[0], t.IsIdentity, scale_data);
                    WriteShortKeyframe(keyframe_writer, t.Rotation[0], t.IsIdentity, rot_data);
                    WriteFloatKeyframe(keyframe_writer, t.Translation[0], t.IsIdentity, trans_data);

                    WriteFloatKeyframe(keyframe_writer, t.Scale[1], t.IsIdentity, scale_data);
                    WriteShortKeyframe(keyframe_writer, t.Rotation[1], t.IsIdentity, rot_data);
                    WriteFloatKeyframe(keyframe_writer, t.Translation[1], t.IsIdentity, trans_data);

                    WriteFloatKeyframe(keyframe_writer, t.Scale[2], t.IsIdentity, scale_data);
                    WriteShortKeyframe(keyframe_writer, t.Rotation[2], t.IsIdentity, rot_data);
                    WriteFloatKeyframe(keyframe_writer, t.Translation[2], t.IsIdentity, trans_data);
                }

                Util.StreamUtility.PadStreamWithString(keyframe_writer, 32);

                ScaleOffset = (int)(keyframe_writer.BaseStream.Position + 0x40);
                foreach (float f in scale_data)
                    keyframe_writer.Write(f);

                Util.StreamUtility.PadStreamWithString(keyframe_writer, 32);

                RotOffset = (int)(keyframe_writer.BaseStream.Position + 0x40);
                foreach (short s in rot_data)
                    keyframe_writer.Write(s);

                Util.StreamUtility.PadStreamWithString(keyframe_writer, 32);

                TransOffset = (int)(keyframe_writer.BaseStream.Position + 0x40);
                foreach (float f in trans_data)
                    keyframe_writer.Write(f);

                keyframe_data = mem.ToArray();
            }

            ScaleCount = scale_data.Count;
            RotCount = rot_data.Count;
            TransCount = trans_data.Count;

            return keyframe_data;
        }

        private void WriteFloatKeyframe(EndianBinaryWriter writer, Keyframe[] keys, bool is_identity, List<float> float_data)
        {
            if (is_identity) // Identity keyframes are easy because they always point to the first value in the list
            {
                writer.Write((short)1); // Number of keys
                writer.Write((short)0); // Index of first key
                writer.Write((short)0); // Tangent type, either piecewise or symmetric

                return;
            }

            writer.Write((short)keys.Length);
            writer.Write((short)float_data.Count);
            writer.Write((short)TangentMode.Symmetric);

            foreach (Keyframe k in keys)
            {
                float_data.Add(k.Time * 30.0f);
                float_data.Add(k.Key);
                float_data.Add(k.InTangent);
            }
        }

        private void WriteShortKeyframe(EndianBinaryWriter writer, Keyframe[] keys, bool is_identity, List<short> short_data)
        {
            if (is_identity)
            {
                writer.Write((short)1);
                writer.Write((short)0);
                writer.Write((short)0);

                return;
            }

            writer.Write((short)keys.Length);
            writer.Write((short)short_data.Count);
            writer.Write((short)TangentMode.Symmetric);

            foreach (Keyframe k in keys)
            {
                short_data.Add((short)(k.Time * 30.0f));
                short_data.Add(RotationFloatToShort(k.Key, RotationFrac));
                short_data.Add(RotationFloatToShort(k.InTangent, RotationFrac));
            }
        }
    }
}
