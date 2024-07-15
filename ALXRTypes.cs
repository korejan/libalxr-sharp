using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace LibALXR
{
    #region LibALXR vec/quaterion/pose types
    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRPosef
    {
        public ALXRQuaternionf orientation;
        public ALXRVector3f position;

        public override string ToString() => $"({orientation},{position})";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRQuaternionf
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public double Yaw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Atan2(2.0 * (y * z + w * x), w * w - x * x - y * y + z * z);
        }

        public double Pitch
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Asin(-2.0 * (x * z - w * y));
        }

        public double Roll
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Math.Atan2(2.0 * (w * x + y * z), 1.0 - 2.0 * (x * x + y * y));
        }

        public override string ToString() => $"({x},{y},{x},{w})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Quaternion(ALXRQuaternionf q) =>
            new System.Numerics.Quaternion(q.x, q.y, q.z, q.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ALXRQuaternionf(System.Numerics.Quaternion q) =>
            new ALXRQuaternionf() { x = q.X, y = q.Y, z = q.Z, w = q.W };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRVector3f
    {
        public float x;
        public float y;
        public float z;

        public override string ToString() => $"({x},{y},{x})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Vector3(ALXRVector3f v) =>
            new System.Numerics.Vector3(v.x, v.y, v.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ALXRVector3f(System.Numerics.Vector3 v) =>
            new ALXRVector3f() { x = v.X, y = v.Y, z = v.Z };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRVector2f
    {
        public float x;
        public float y;

        public override string ToString() => $"({x},{y})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Vector2(ALXRVector2f v) =>
            new System.Numerics.Vector2(v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ALXRVector2f(System.Numerics.Vector2 v) =>
            new ALXRVector2f() { x = v.X, y = v.Y };
    }
    #endregion

    #region You can ignore these
    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRTrackingInfo
    {
        public const uint MAX_CONTROLLERS = 2;
        public const uint BONE_COUNT = 19;

        [StructLayout(LayoutKind.Sequential)]
        public struct Controller
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
            public ALXRQuaternionf[] boneRotations;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
            public ALXRVector3f[] bonePositionsBase;
            public ALXRPosef boneRootPose;

            public ALXRPosef pose;
            public ALXRVector3f angularVelocity;
            public ALXRVector3f linearVelocity;

            public ALXRVector2f trackpadPosition;

            public ulong buttons;

            public float triggerValue;
            public float gripValue;

            public uint handFingerConfidences;

            [MarshalAs(UnmanagedType.U1)]
            public bool enabled;
            [MarshalAs(UnmanagedType.U1)]
            public bool isHand;
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Controller[] controller;

        public ALXRPosef headPose;
        public ulong targetTimestampNs;
        public byte mounted;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRTimeSync
    {
        public uint type;
        public uint mode;
        public ulong sequence;
        public ulong serverTime;
        public ulong clientTime;
        public ulong packetsLostTotal;
        public ulong packetsLostInSecond;
        public ulong averageDecodeLatency;
        public uint averageTotalLatency;
        public uint averageSendLatency;
        public uint averageTransportLatency;
        public uint idleTime;
        public ulong fecFailureInSecond;
        public ulong fecFailureTotal;
        public uint fecFailure;
        public float fps;
        public ulong trackingRecvFrameIndex;
        public uint serverTotalLatency;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXREyeFov
    {
        public float left;
        public float right;
        public float top;
        public float bottom;
    }
    #endregion

    #region LibALXR C-types
    public enum ALXRGraphicsApi : uint
    {
        [EnumMember(Value = "Auto")]
        Auto,
        [EnumMember(Value = "Vulkan2")]
        Vulkan2,
        [EnumMember(Value = "Vulkan")]
        Vulkan,
        [EnumMember(Value = "D3D12")]
        D3D12,
        [EnumMember(Value = "D3D11")]
        D3D11,
        ApiCount = D3D11
    }

    public enum ALXRDecoderType : uint
    {
        D311VA,
        NVDEC,
        CUVID,
        VAAPI,
        CPU
    }

    public enum ALXRTrackingSpace : uint
    {
        LocalRefSpace,
        StageRefSpace,
        ViewRefSpace
    }

    public enum ALXRCodecType : uint
    {
        H264_CODEC,
        HEVC_CODEC
    }

    // replicates https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_FB_color_space
    public enum ALXRColorSpace : int
    {
        Unmanaged = 0,
        Rec2020 = 1,
        Rec709 = 2,
        RiftCV1 = 3,
        RiftS = 4,
        Quest = 5,
        P3 = 6,
        AdobeRgb = 7,
        Default = Quest,
        MaxEnum = 0x7fffffff
    }

    [Flags]
    public enum ALXRTrackingEnabledFlags : ulong
    {
        Hands = (1u << 0),
        Eyes = (1u << 1),
        Face = (1u << 2),
        All = ALXRTrackingEnabledFlags.Hands | ALXRTrackingEnabledFlags.Eyes | ALXRTrackingEnabledFlags.Face
    }

    public enum ALXRPassthroughMode : uint {
        None = 0,
        BlendLayer,
        MaskLayer,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ALXRSystemProperties
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string systemName;
        public float currentRefreshRate;
        public IntPtr refreshRates;
        [MarshalAs(UnmanagedType.U4)]
        public uint refreshRatesCount;
        [MarshalAs(UnmanagedType.U4)]
        public uint recommendedEyeWidth;
        [MarshalAs(UnmanagedType.U4)]
        public uint recommendedEyeHeight;
        [MarshalAs(UnmanagedType.U8)]
        public ALXRTrackingEnabledFlags enabledTrackingSystemsFlags;

        public bool IsHandTrackingEnabled => (enabledTrackingSystemsFlags & ALXRTrackingEnabledFlags.Hands) != 0;
        public bool IsEyeTrackingEnabled => (enabledTrackingSystemsFlags & ALXRTrackingEnabledFlags.Eyes) != 0;
        public bool IsFaceTrackingEnabled => (enabledTrackingSystemsFlags & ALXRTrackingEnabledFlags.Face) != 0;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXREyeInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ALXREyeFov[] eyeFov;
        public float ipd;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRVersion
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint major;
        [MarshalAs(UnmanagedType.U4)]
        public uint minor;
        [MarshalAs(UnmanagedType.U4)]
        public uint patch;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRClientCtx
    {
        public const CallingConvention ALXRCallingConvention = CallingConvention.Cdecl;

        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void InputSendDelegate(ref ALXRTrackingInfo data);
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void ViewsConfigSendDelegate(ref ALXREyeInfo eyeInfo);
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate ulong PathStringToHashDelegate(string path);
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void TimeSyncSendDelegate(ref ALXRTimeSync data);
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void VideoErrorReportSendDelegate();
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void BatterySendDelegate(ulong device_path, float gauge_value, bool is_plugged);
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void SetWaitingNextIDRDelegate(bool waiting);
        [UnmanagedFunctionPointer(ALXRCallingConvention)]
        public delegate void RequestIDRDelegate();

        public InputSendDelegate inputSend;
        public ViewsConfigSendDelegate viewsConfigSend;
        public PathStringToHashDelegate pathStringToHash;
        public TimeSyncSendDelegate timeSyncSend;
        public VideoErrorReportSendDelegate videoErrorReportSend;
        public BatterySendDelegate batterySend;
        public SetWaitingNextIDRDelegate setWaitingNextIDR;
        public RequestIDRDelegate requestIDR;

        public ALXRVersion firmwareVersion;
        [MarshalAs(UnmanagedType.U4)]
        public ALXRGraphicsApi graphicsApi;
        [MarshalAs(UnmanagedType.U4)]
        public ALXRDecoderType decoderType;
        [MarshalAs(UnmanagedType.I4)]
        public ALXRColorSpace displayColorSpace;
        [MarshalAs(UnmanagedType.U4)]
        public ALXRPassthroughMode passthroughMode;

        //
        // internalDataPath - Optional root path of data files, if null all paths will be relative, 
        // for android builds this could be the same as ANativeActivity::internalDataPath
        //
        public IntPtr internalDataPath;
        public string InternalDataPath => Marshal.PtrToStringAnsi(internalDataPath);

        [MarshalAs(UnmanagedType.U4)]
        public uint faceTrackingDataSources;
        [MarshalAs(UnmanagedType.U1)]
        public ALXRFacialExpressionType facialTracking;
        [MarshalAs(UnmanagedType.U1)]
        public ALXREyeTrackingType eyeTracking;

        [MarshalAs(UnmanagedType.U2)]
        public ushort trackingServerPortNo;

        [MarshalAs(UnmanagedType.U1)]
        public bool verbose;
        [MarshalAs(UnmanagedType.U1)]
        public bool disableLinearizeSrgb;
        [MarshalAs(UnmanagedType.U1)]
        public bool noSuggestedBindings;
        [MarshalAs(UnmanagedType.U1)]
        public bool noServerFramerateLock;
        [MarshalAs(UnmanagedType.U1)]
        public bool noFrameSkip;
        [MarshalAs(UnmanagedType.U1)]
        public bool disableLocalDimming;
        [MarshalAs(UnmanagedType.U1)]
        public bool headlessSession;
        [MarshalAs(UnmanagedType.U1)]
        public bool noFTServer;
        [MarshalAs(UnmanagedType.U1)]
        public bool noPassthrough;
        [MarshalAs(UnmanagedType.U1)]
        public bool noHandTracking;
        // Enables a headless OpenXR session if supported by the runtime (same as `headlessSession`).
        // In the absence of native support, will attempt to simulate a headless session.
        // Caution: May not be compatible with all runtimes and could lead to unexpected behavior.
        [MarshalAs(UnmanagedType.U1)]
        public bool simulateHeadless;

#if XR_USE_PLATFORM_ANDROID
        public IntPtr applicationVM;
        public IntPtr applicationActivity;
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRGuardianData
    {
        public float areaWidth;
        public float areaHeight;
        [MarshalAs(UnmanagedType.U1)]
        public bool shouldSync;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRRenderConfig
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint eyeWidth;
        [MarshalAs(UnmanagedType.U4)]
        public uint eyeHeight;
        public float refreshRate;
        public float foveationCenterSizeX;
        public float foveationCenterSizeY;
        public float foveationCenterShiftX;
        public float foveationCenterShiftY;
        public float foveationEdgeRatioX;
        public float foveationEdgeRatioY;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableFoveation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRDecoderConfig
    {
        [MarshalAs(UnmanagedType.U4)]
        public ALXRCodecType codecType;
        [MarshalAs(UnmanagedType.U4)]
        public uint cpuThreadCount; // only used for software decoding.
        [MarshalAs(UnmanagedType.U1)]
        public bool enableFEC;
        [MarshalAs(UnmanagedType.U1)]
        public bool realtimePriority;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ALXRStreamConfig
    {
        [MarshalAs(UnmanagedType.U4)]
        public ALXRTrackingSpace trackingSpaceType;
        public ALXRRenderConfig renderConfig;
        public ALXRDecoderConfig decoderConfig;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ALXRProcessFrameResult
    {
        public ALXRHandTracking handTracking;
        public ALXRFacialEyePacket facialEyeTracking;

        [MarshalAs(UnmanagedType.U1)]
        public bool exitRenderLoop;
        [MarshalAs(UnmanagedType.U1)]
        public bool requestRestart;
    }

    [Flags]
    public enum ALXRLogOptions : uint
    {
        None = 0,
        Timestamp = (1u << 0),
        LevelTag = (1u << 1)
    }

    public enum ALXRLogLevel : uint
    {
        Verbose,
        Info,
        Warning,
        Error
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ALXRLogOutputFn(ALXRLogLevel level, string output, uint len);
    #endregion
}
