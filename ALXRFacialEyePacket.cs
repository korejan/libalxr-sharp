using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System;

namespace LibALXR
{
    using XrPosef = ALXRPosef;

    public enum ALXRFacialExpressionType : byte
    {
        [EnumMember(Value = "None")]
        None = 0, // Not Support or Disabled
        [EnumMember(Value = "FB")]
        FB,
        [EnumMember(Value = "FB_V2")]
        FB_V2,
        [EnumMember(Value = "HTC")]
        HTC,
        [EnumMember(Value = "Pico")] // future support.
        Pico,
        [EnumMember(Value = "Auto")]
        Auto,
        TypeCount
    }

    public enum ALXREyeTrackingType : byte
    {
        [EnumMember(Value = "None")]
        None = 0, // Not Support or Disabled
        [EnumMember(Value = "FBEyeTrackingSocial")]
        FBEyeTrackingSocial,
        [EnumMember(Value = "ExtEyeGazeInteraction")]
        ExtEyeGazeInteraction,
        [EnumMember(Value = "Auto")]
        Auto,
        TypeCount
    }

    public enum ALXRFaceTrackingDataSource : byte
    {
        [EnumMember(Value = "VisualSource")]
        VisualSource = 0,
        [EnumMember(Value = "AudioSource")]
        AudioSource,
        [EnumMember(Value = "UnknownSource")]
        UnknownSource,
        TypeCount
    };

    [Flags]
    public enum ALXRFaceTrackingDataSourceFlags : uint
    {
        VisualSource = (1u << 0),
        AudioSource = (1u << 1),
        AnySource = VisualSource | AudioSource
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ALXRFacialEyePacket
    {
        public const int MaxEyeCount = 2;
        public const int MaxExpressionCount = 70;

        [MarshalAs(UnmanagedType.U1)]
        public ALXRFacialExpressionType expressionType;
        [MarshalAs(UnmanagedType.U1)]
        public ALXREyeTrackingType eyeTrackerType;
        [MarshalAs(UnmanagedType.U1)]
        public byte isEyeFollowingBlendshapesValid;
        public unsafe fixed byte isEyeGazePoseValid[MaxEyeCount];
        [MarshalAs(UnmanagedType.U1)]
        public ALXRFaceTrackingDataSource expressionDataSource;
        public unsafe fixed float expressionWeights[MaxExpressionCount];

        public XrPosef eyeGazePose0;
        public XrPosef eyeGazePose1;

        public ReadOnlySpan<float> ExpressionWeightSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                unsafe
                {
                    fixed (float* p = expressionWeights)
                    {
                        return new ReadOnlySpan<float>(p, MaxExpressionCount);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ALXRFacialEyePacket ReadPacket(byte[] array)
        {
            var newPacket = new ALXRFacialEyePacket();
            ReadPacket(array, ref newPacket);
            return newPacket;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadPacket(byte[] array, ref ALXRFacialEyePacket newPacket)
        {
            Debug.Assert(array.Length >= Marshal.SizeOf<ALXRFacialEyePacket>());
            ReadMemoryMarshal(array, 0, array.Length, out newPacket);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadMemoryMarshal<T>(byte[] array, int offset, int size, out T result) where T : struct
        {
            result = MemoryMarshal.Cast<byte, T>(array.AsSpan(offset, size))[0];
        }
    }
}