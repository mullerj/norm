using Mil.Navy.Nrl.Norm.Enums;
using System.Runtime.InteropServices;

namespace Mil.Navy.Nrl.Norm
{
    /// <summary>
    /// The native NORM API functions 
    /// </summary>
    public static class NormApi
    {
        public const string NORM_LIBRARY = "norm";
        public const int NORM_SESSION_INVALID = 0;
        public const int NORM_OBJECT_INVALID = 0;
        public const int NORM_NODE_INVALID = 0;
        public const int NORM_DESCRIPTOR_INVALID = 0;
        public const int FILENAME_MAX = 260;

        [StructLayout(LayoutKind.Sequential)]
        public struct NormEvent
        {
            public NormEventType Type;
            public long Session;
            public long Sender;
            public long Object;
        }

        /// <summary>
        /// This function creates an instance of a NORM protocol engine and is the necessary first step before any other API functions may be used.
        /// </summary>
        /// <param name="priorityBoost">The priorityBoost parameter, when set to a value of true, specifies that the NORM protocol engine thread be run with higher priority scheduling.</param>
        /// <returns>A value of NORM_INSTANCE_INVALID is returned upon failure.</returns>
        [DllImport(NORM_LIBRARY)]
        public static extern long NormCreateInstance(bool priorityBoost);

        /// <summary>
        /// The NormDestroyInstance() function immediately shuts down and destroys the NORM protocol engine instance referred to by the instanceHandle parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter</param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormDestroyInstance(long instanceHandle);

        /// <summary>
        /// The NormStopInstance() this function immediately stops the NORM protocol engine thread corresponding to the given instanceHandler parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter</param>
        [DllImport (NORM_LIBRARY)]
        public static extern void NormStopInstance(long instanceHandle);
      
        /// <summary>
        /// The NormRestartInstance() this function creates and starts an operating system threadto resume NORM protocol engine operation for the given instanceHandle that was previously stopped by a call to NormStopInstance().
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter</param>
        [DllImport (NORM_LIBRARY)]
        public static extern bool NormRestartInstance(long instanceHandle);

        /// <summary>
        /// This function creates a NORM protocol session (NormSession) using the address (multicast or unicast) and port
        /// parameters provided.While session state is allocated and initialized, active session participation does not begin
        /// until a call is made to NormStartSender() and/or NormStartReceiver() to join the specified multicast group
        /// (if applicable) and start protocol operation.
        /// </summary>
        /// <param name="instanceHandle">Valid NormInstanceHandle previously obtained with a call to NormCreateInstance() </param>
        /// <param name="sessionAddress">Specified address determines the destination of NORM messages sent </param>
        /// <param name="sessionPort">Valid, unused port number corresponding to the desired NORM session address. </param>
        /// <param name="localNodeId">Identifies the application's presence in the NormSession </param>
        /// <returns></returns>
        [DllImport(NORM_LIBRARY)]
        public static extern long NormCreateSession(long instanceHandle, string sessionAddress, int sessionPort, long localNodeId);

        /// <summary>
        /// This function immediately terminates the application's participation in the NormSession and frees any resources used by that session.
        /// </summary>
        /// <param name="sessionHandle"> Used to identify application in the NormSession </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormDestroySession(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetLoopback(long sessionHandle, bool loopback);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormGetRandomSessionId();

        /// <summary>
        /// The application's participation as a sender within a specified NormSession begins when this function is called.
        /// </summary>
        /// <param name="instanceHandle"> Valid NormSessionHandle previously obtained with a call to NormCreateSession() </param>
        /// <param name="instanceId"> Application-defined value used as the instance_id field of NORM sender messages for the application's participation within a session </param>
        /// <param name="bufferSpace"> This specifies the maximum memory space (in bytes) the NORM protocol engine is allowed to use to buffer any sender calculated FEC segments and repair state for the session. </param>
        /// <param name="segmentSize"> This parameter sets the maximum payload size (in bytes) of NORM sender messages (not including any NORM message header fields). </param>
        /// <param name="numData">  </param>
        /// <param name="numParity"></param>
        /// <param name="fedId"></param>
        /// <returns></returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStartSender(long instanceHandle, int instanceId, long bufferSpace, int segmentSize, short numData, short numParity, NormFecType fedId);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStopSender(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormFileEnqueue(long sessionHandle, string fileName);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormFileEnqueue(long sessionHandle, string fileName, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormStreamOpen(long sessionHandle, long bufferSize, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        internal static extern int NormStreamWrite(long streamHandle, byte[] buffer, int numBytes);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamMarkEom(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamFlush(long streamHandle, bool eom, NormFlushMode flushMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamClose(long streamHandle, bool graceful);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStreamRead(long streamHandle, byte[] buffer, ref int numBytes);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormGetDescriptor(long instanceHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormGetNextEvent(long instanceHandle, out NormEvent theEvent);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormGetNextEvent(long instanceHandle, out NormEvent theEvent, bool waitForEvent);

        [DllImport(NORM_LIBRARY)]
        public static extern NormObjectType NormObjectGetType(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormFileRename(long fileHandle, string fileName);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormFileGetName(long fileHandle, [Out] char[] nameBuffer, int bufferLen);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStartReceiver(long sessionHandle, long bufferSpace);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStopReceiver(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetCacheDirectory(long instanceHandle, string cachePath);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormDataEnqueue(long sessionHandle, string dataPtr, int dataLen);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormDataEnqueue(long sessionHandle, string dataPtr, int dataLen, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        public static extern IntPtr NormDataAccessData(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetSize(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern string NormDataDetachData(long objectHandle);
    }
}
