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
        public const int NORM_NODE_NONE = 0;
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
        /// The NormSuspendtInstance()
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter</param>
        [DllImport (NORM_LIBRARY)]
        public static extern bool NormSuspendInstance(long instanceHandle);

        [DllImport (NORM_LIBRARY)]
        public static extern bool NormResumeInstance(long instanceHandle);

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

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormOpenDebugLog(long instanceHandle, string path);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormCloseDebugLog(long instanceHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDebugLevel(int level);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormGetDebugLevel();

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
        public static extern bool NormObjectHasInfo(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetInfoLength(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetInfo(long objectHandle, [Out] byte[] buffer, int bufferLen);

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
        public static extern long NormDataEnqueue(long sessionHandle, byte[] dataPtr, int dataLen);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormDataEnqueue(long sessionHandle, byte[] dataPtr, int dataLen, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        public static extern nint NormDataAccessData(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetSize(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern nint NormDataDetachData(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTxPort(long sessionHandle, int txPortNumber, bool enableReuse, string? txBindAddress);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetRxPortReuse(long sessionHandle, bool enableReuse, string? rxBindAddress, string? senderAddress, int senderPort);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetEcnSupport(long sessionHandle, bool ecnEnable, bool ignoreLoss, bool tolerateLoss);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetMulticastInterface(long sessionHandle, string interfaceName);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetSSM(long sessionHandle, string sourceAddress);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTTL(long sessionHandle, byte ttl);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTOS(long sessionHandle, byte tos);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetMessageTrace(long sessionHandle, bool flag);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxLoss(long sessionHandle, double precent);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetRxLoss(long sessionHandle, double precent);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetReportInterval(long sessionHandle, double interval);

        [DllImport(NORM_LIBRARY)]
        public static extern double NormGetReportInterval(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxOnly(long sessionHandle, bool txOnly, bool connectToSessionAddress);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetFlowControl(long sessionHandle, double flowControlFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTxSocketBuffer(long sessionHandle, long bufferSize);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetCongestionControl(long sessionHandle, bool enable, bool adjustRate);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxRateBounds(long sessionHandle, double rateMin, double rateMax);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxCacheBounds(long sessionHandle, long sizeMax, long countMin, long countMax);

        [DllImport(NORM_LIBRARY)]
        public static extern double NormGetTxRate(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxRate(long sessionHandle, double rate);

        [DllImport(NORM_LIBRARY)]
        public static extern double NormGetGrttEstimate(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetGrttEstimate(long sessionHandle, double grtt);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetAutoParity(long sesssionHandle, short autoParity);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetGrttMax(long sessionHandle, double grttMax);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetGrttProbingMode(long sesssionHandle, NormProbingMode probingMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetGrttProbingInterval(long sessionHandle, double intervalMin, double intervalMax);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetBackoffFactor(long sessionHandle, double backoffFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetGroupSize(long sessiionHandle, long groupSize);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxRobustFactor(long sessionHandle, int txRobustFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormRequeueObject(long sessionHandle, long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetWatermark(long sessionHandle, long objectHandle, bool overrideFlush);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormCancelWatermark(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormResetWatermark(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormAddAckingNode(long sessionHandle, long nodeId);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormRemoveAckingNode(long sessionHandle, long nodeId);

        [DllImport(NORM_LIBRARY)]
        public static extern NormAckingStatus NormGetAckingStatus(long sessionHandle, long nodeId);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSendCommand(long sessionHandle, byte[] cmdBuffer, int cmdLength, bool robust);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormCancelCommand(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetRxCacheLimit(long sessionHandle, int countMax);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetRxSocketBuffer(long sessionHandle, long bufferSize);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetSilentReceiver(long sessionHandle, bool silent, int maxDelay);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultUnicastNack(long sessionHandle, bool enable);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetUnicastNack(long remoteSender, bool unicastNacks);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultSyncPolicy(long sessionHandle, NormSyncPolicy syncPolicy);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultNackingMode(long sessionHandle, NormNackingMode nackingMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetNackingMode(long remoteSender, NormNackingMode nackingMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultRepairBoundary(long sessionHandle, NormRepairBoundary repairBoundary);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetRepairBoundary(long remoteSender, NormRepairBoundary repairBoundary);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultRxRobustFactor(long sessionHandle, int robustFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetRxRobustFactor(long remoteSender, int robustFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormGetLocalNodeId(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormNodeGetId(long nodeHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormNodeGetAddress(long nodeHandle, [Out] byte[] addrBuffer, ref int bufferLen, out int port);

        [DllImport(NORM_LIBRARY)]
        public static extern double NormNodeGetGrtt(long nodeHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormNodeGetCommand(long remoteSender, [Out] byte[] buffer, ref int buflen);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeFreeBuffers(long remoteSender);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeRetain(long nodeHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeRelease(long nodeHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectSetNackingMode(long objectHandle, NormNackingMode nackingMode);
        
        [DllImport(NORM_LIBRARY)]
        public static extern long NormObjectGetBytesPending(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectCancel(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectRetain(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectRelease(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormObjectGetSender(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStreamHasVacancy(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormStreamGetReadOffset(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStreamSeekMsgStart(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamSetPushEnable(long streamHandle, bool pushEnable);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamSetAutoFlush(long streamHandle, NormFlushMode flushMode);
    }
}
