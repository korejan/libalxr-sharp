using System;
using System.Threading;

namespace LibALXR.examples
{
    internal static class Local
    {
        internal class Context
        {
            public string DLLDir => "";

            public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

            public ALXRGraphicsApi GraphicsApi { get; set; } = ALXRGraphicsApi.Auto;

            public ALXRFacialExpressionType FacialTrackingExt { get; set; } = ALXRFacialExpressionType.Auto;

            public ALXREyeTrackingType EyeTrackingExt { get; set; } = ALXREyeTrackingType.Auto;

            public bool EnableHandleTracking { get; set; } = true;

            // If you only care about tracking data, set this true but
            // is only enabled if a particular runtime supports `XR_MND_headless` extension.
            public bool HeadlessSession { get; set; } = true;

            // Enables a headless OpenXR session if supported by the runtime (same as `HeadlessSession`).
            // In the absence of native support, will attempt to simulate a headless session.
            // Caution: May not be compatible with all runtimes and could lead to unexpected behavior.
            public bool SimulateHeadless { get; set; } = true;

            public bool VerboseLogs { get; set; } = false;
        }

        public static void Run(Context runCtx)
        {
            if (runCtx == null)
            {
                Console.Error.WriteLine("run context is null.");
                return;
            }

            if (!LibALXR.AddDllSearchPath(runCtx.DLLDir))
            {
                Console.Error.WriteLine($"[libalxr] unmanaged library path to search failed to be set.");
                return;
            }

            try
            {
                LibALXR.alxr_set_log_custom_output(ALXRLogOptions.None, (level, output, len) =>
                {
                    var fullMsg = $"[libalxr] {output}";
                    switch (level)
                    {
                        case ALXRLogLevel.Info:
                        case ALXRLogLevel.Warning:
                            Console.Out.WriteLine(fullMsg);
                            break;
                        case ALXRLogLevel.Error:
                            Console.Error.WriteLine(fullMsg);
                            break;
                    }
                });

                while (!runCtx.CancellationToken.IsCancellationRequested)
                {
                    var ctx = CreateALXRClientCtx(runCtx);
                    var sysProperties = new ALXRSystemProperties();
                    if (!LibALXR.alxr_init(ref ctx, out sysProperties))
                    {
                        break;
                    }

                    PrintSystemProperties(ref sysProperties);

                    var processFrameResult = new ALXRProcessFrameResult
                    {
                        handTracking = new ALXRHandTracking(),
                        facialEyeTracking = new ALXRFacialEyePacket(),
                        exitRenderLoop = false,
                        requestRestart = false,
                    };
                    while (!runCtx.CancellationToken.IsCancellationRequested)
                    {
                        processFrameResult.exitRenderLoop = false;
                        LibALXR.alxr_process_frame2(ref processFrameResult);
                        if (processFrameResult.exitRenderLoop)
                        {
                            break;
                        }

                        // do something with processFrameResult result

                        if (!LibALXR.alxr_is_session_running())
                        {
                            // Throttle loop since xrWaitFrame won't be called.
                            Thread.Sleep(250);
                        }
                    }

                    LibALXR.alxr_destroy();

                    if (!processFrameResult.requestRestart)
                    {
                        break;
                    }
                }
            }
            finally
            {
                LibALXR.alxr_destroy();
            }
        }

        private static void PrintSystemProperties(ref ALXRSystemProperties sysProperties)
        {
            Console.WriteLine($"Runtime Name: {sysProperties.systemName}");
            Console.WriteLine($"Hand-tracking enabled? {sysProperties.IsHandTrackingEnabled}");
            Console.WriteLine($"Eye-tracking enabled? {sysProperties.IsEyeTrackingEnabled}");
            Console.WriteLine($"Face-tracking enabled? {sysProperties.IsHandTrackingEnabled}");
        }

        private static ALXRClientCtx CreateALXRClientCtx(Context config)
        {
            return new ALXRClientCtx
            {
                inputSend = (ref ALXRTrackingInfo data) => { },
                viewsConfigSend = (ref ALXREyeInfo eyeInfo) => { },
                pathStringToHash = (path) => { return (ulong)path.GetHashCode(); },
                timeSyncSend = (ref ALXRTimeSync data) => { },
                videoErrorReportSend = () => { },
                batterySend = (a, b, c) => { },
                setWaitingNextIDR = a => { },
                requestIDR = () => { },
                graphicsApi = config.GraphicsApi,
                decoderType = ALXRDecoderType.D311VA,
                displayColorSpace = ALXRColorSpace.Default,
                faceTrackingDataSources = (uint)ALXRFaceTrackingDataSourceFlags.VisualSource,
                facialTracking = config.FacialTrackingExt,
                eyeTracking = config.EyeTrackingExt,
                trackingServerPortNo = LibALXR.TrackingServerDefaultPortNo,
                verbose = config.VerboseLogs,
                disableLinearizeSrgb = false,
                noSuggestedBindings = true,
                noServerFramerateLock = false,
                noFrameSkip = false,
                disableLocalDimming = true,
                headlessSession = config.HeadlessSession,
                simulateHeadless = config.SimulateHeadless,
                noFTServer = true,
                noPassthrough = true,
                noHandTracking = !config.EnableHandleTracking,
                firmwareVersion = new ALXRVersion
                {
                    // only relevant for android clients.
                    major = 0,
                    minor = 0,
                    patch = 0
                }
            };
        }
    }
}
