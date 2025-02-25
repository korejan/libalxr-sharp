using System.IO;
using System.Runtime.InteropServices;

namespace LibALXR
{
    #region LibALXR Functions
    public static class LibALXR
    {
        public const ushort TrackingServerDefaultPortNo = 49192;

        public const string DllName = "alxr_engine";

        public const CallingConvention ALXRCallingConvention = CallingConvention.Cdecl;


        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern bool alxr_init(ref ALXRClientCtx ctx, out ALXRSystemProperties systemProperties);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_destroy();

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_request_exit_session();

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_process_frame([In, Out] ref bool exitRenderLoop, [In, Out] ref bool requestRestart);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_process_frame2([In, Out] ref ALXRProcessFrameResult result);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern bool alxr_is_session_running();

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_set_stream_config(ALXRStreamConfig config);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern ALXRGuardianData alxr_get_guardian_data();

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_on_receive(System.IntPtr packet, uint packetSize);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_on_tracking_update(bool clientsidePrediction);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_on_haptics_feedback(ulong path, float duration_s, float frequency, float amplitude);

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_on_server_disconnect();

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_on_pause();

        [DllImport(DllName, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_on_resume();
                
        [DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = ALXRCallingConvention)]
        public static extern void alxr_set_log_custom_output(ALXRLogOptions options, ALXRLogOutputFn outputFn);


    }
    #endregion
}