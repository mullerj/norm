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
        /// <param name="priorityBoost">The priorityBoost parameter, when set to a value of true, specifies that the NORM protocol engine thread be run with higher priority scheduling. </param>
        /// <returns>A value of NORM_INSTANCE_INVALID is returned upon failure. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern long NormCreateInstance(bool priorityBoost);

        /// <summary>
        /// The NormDestroyInstance() function immediately shuts down and destroys the NORM protocol engine instance referred to by the instanceHandle parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormDestroyInstance(long instanceHandle);

        /// <summary>
        /// The NormStopInstance() this function immediately stops the NORM protocol engine thread corresponding to the given instanceHandle parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        [DllImport (NORM_LIBRARY)]
        public static extern void NormStopInstance(long instanceHandle);
      
        /// <summary>
        /// The NormRestartInstance() this function creates and starts an operating system thread to resume NORM protocol engine operation for the given instanceHandle that was previously stopped by a call to NormStopInstance().
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        /// <returns>Boolean as to the success of the instance restart. </returns>
        [DllImport (NORM_LIBRARY)]
        public static extern bool NormRestartInstance(long instanceHandle);

        /// <summary>
        /// The NormSuspendInstance() immediately suspends the NORM protocol engine thread corresponding to the given instanceHandle parameter
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        /// <returns>Boolean as to the success of the instance suspension. </returns>
        [DllImport (NORM_LIBRARY)]
        public static extern bool NormSuspendInstance(long instanceHandle);

        /// <summary>
        /// Resumes NORM protocol engine thread corresponding to the given instanceHandler parameter.
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        /// <returns>Boolean as to the success of the instance resumption. </returns>
        [DllImport (NORM_LIBRARY)]
        public static extern bool NormResumeInstance(long instanceHandle);

        /// <summary>
        /// This function sets the directory path used by receivers to cache newly-received NORM_OBJECT_FILE content.
        /// </summary>
        /// <param name="instanceHandle">The instanceHandle parameter specifies the NORM protocol engine instance (all NormSessions associated with that instanceHandle share the same cache path). </param>
        /// <param name="cachePath">the cachePath is a string specifying a valid (and writable) directory path. </param>
        /// <returns>The function returns true on success and false on failure. The failure conditions are that the indicated directory does not exist or the process does not have permissions to write. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetCacheDirectory(long instanceHandle, string cachePath);

        /// <summary>
        /// This function retrieves the next available NORM protocol event from the protocol engine.
        /// </summary>
        /// <param name="instanceHandle"The instanceHandle parameter specifies the applicable NORM protocol engine. </param>
        /// <param name="theEvent"> the theEvent parameter must be a valid pointer to a NormEvent structure capable of receiving the NORM event information. </param>
        /// <param name="waitForEvent">waitForEvent specifies whether the call to this function is blocking or not, if "waitForEvent" is false, this is a non-blocking call. </param>
        /// <returns>The function returns true when a NormEvent is successfully retrieved, and false otherwise. Note that a return value of false does not indicate an error or signify end of NORM operation.</returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormGetNextEvent(long instanceHandle, out NormEvent theEvent, bool waitForEvent);

        /// <summary>
        /// This function is used to retrieve a NormDescriptor (Unix int file descriptor or Win32 HANDLE) suitable for
        /// asynchronous I/O notification to avoid blocking calls to NormGetNextEvent().
        /// </summary>
        /// <param name="instanceHandle">The NORM protocol engine instance referred to by the instanceHandle parameter. </param>
        /// <returns>A NormDescriptor value is returned which is valid until a call to NormDestroyInstance() is made. Upon error, a value of NORM_DESCRIPTOR_INVALID is returned </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern int NormGetDescriptor(long instanceHandle);

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
        /// <param name="sessionHandle"> Used to identify application in the NormSession. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormDestroySession(long sessionHandle);

        /// <summary>
        /// This function retrieves the NormNodeId value used for the application's participation in the NormSession.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <returns>The returned value indicates the NormNode identifier used by the NORM protocol engine for the local application's participation in the specified NormSession. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern long NormGetLocalNodeId(long sessionHandle);

        /// <summary>
        /// This function is used to force NORM to use a specific port number for UDP packets sent for the specified sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="txPortNumber">The txPortNumber parameter, specifies which port number to use. </param>
        /// <param name="enableReuse">The enableReuse parameter, when set to true, allows that the specified port may be reused for multiple sessions. </param>
        /// <param name="txBindAddress">. The txBindAddress parameter allows specification of a specific source address binding for packet transmission. </param>
        /// <returns>This function returns true upon success and false upon failure. Failure will occur if a txBindAddress is providedthat does not correspond to a valid, configured IP address for the local host system.</returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTxPort(long sessionHandle, int txPortNumber, bool enableReuse, string? txBindAddress);

        /// <summary>
        /// This function limits the NormSession to perform NORM sender functions only.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="txOnly">Boolean specifing whether to turn on or off the txOnly operation. </param>
        /// <param name="connectToSessionAddress">The optional connectToSessionAddress parameter, when set to true, causes the underlying NORM code to "connect()" the UDP socket to the session (remote receiver) address and port number. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxOnly(long sessionHandle, bool txOnly, bool connectToSessionAddress);

        /// <summary>
        /// This function allows the user to control the port reuse and binding behavior for the receive socket used for the given NORM sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="enableReuse">When the enablReuse parameter is set to true, reuse of the NormSession port number by multiple NORM instances or sessions is enabled. </param>
        /// <param name="rxBindAddress">If the optional rxBindAddress is supplied (an IP address or host name in string form), the socket will bind() to the given address when it is opened in a call to NormStartReceiver() or NormStartSender(). </param>
        /// <param name="senderAddress">The optional senderAddress parameter can be used to connect() the underlying NORM receive socket to specific address. </param>
        /// <param name="senderPort">The optional senderPort parameter can be used to connect() the underlying NORM receive socket to specific port. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetRxPortReuse(long sessionHandle, bool enableReuse, string? rxBindAddress, string? senderAddress, int senderPort);

        /// <summary>
        /// (TBD - Describe the NormSetEcnSupport() function as this experimental option matures.)
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="ecnEnable"></param>
        /// <param name="ignoreLoss"></param>
        /// <param name="tolerateLoss"></param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetEcnSupport(long sessionHandle, bool ecnEnable, bool ignoreLoss, bool tolerateLoss);

        /// <summary>
        /// This function specifies which host network interface is used for IP Multicast transmissions and group membership.
        /// This should be called before any call to NormStartSender() or NormStartReceiver() is made so that the IP multicast group is joined on the proper host interface.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="interfaceName"></param>
        /// <returns>A return value of true indicates success while a return value of false indicates that the specified interface was
        /// invalid. This function will always return true if made before calls to NormStartSender() or NormStartReceiver().
        /// However, those calls may fail if an invalid interface was specified with the call described here. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetMulticastInterface(long sessionHandle, string interfaceName);

        /// <summary>
        /// This function sets the source address for Source-Specific Multicast (SSM) operation.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="sourceAddress"></param>
        /// <returns>A return value of true indicates success while a return value of false indicates that the specified source address
        /// was invalid. Note that if a valid IP address is specified but is improper for SSM (e.g., an IP multicast address) the
        /// later calls to NormStartSender() or NormStartReceiver() may fail. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetSSM(long sessionHandle, string sourceAddress);

        /// <summary>
        /// This function specifies the time-to-live (ttl) for IP Multicast datagrams generated by NORM for the specified
        /// sessionHandle. The IP TTL field limits the number of router "hops" that a generated multicast packet may traverse
        /// before being dropped.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="ttl">If TTL is equal to one, the transmissions will be limited to the local area network
        /// (LAN) of the host computers network interface. Larger TTL values should be specified to span large networks.</param>
        /// <returns>A return value of true indicates success while a return value of false indicates that the specified ttl could not
        /// be set. This function will always return true if made before calls to NormStartSender() or NormStartReceiver().
        /// However, those calls may fail if the desired ttl value cannot be set.</returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTTL(long sessionHandle, byte ttl);

        /// <summary>
        /// This function specifies the type-of-service (tos) field value used in IP Multicast datagrams generated by NORM for the specified sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="tos">The IP TOS field value can be used as an indicator that a "flow" of packets may merit special Quality-of-Service (QoS) treatment by network devices.
        /// Users should refer to applicable QoS information for their network to determine the expected interpretation and treatment (if any) of packets with explicit TOS marking.</param>
        /// <returns>A return value of true indicates success while a return value of false indicates that the specified tos could not
        /// be set. This function will always return true if made before calls to NormStartSender() or NormStartReceiver().
        /// However, those calls may fail if the desired tos value cannot be set.</returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTOS(long sessionHandle, byte tos);

        /// <summary>
        /// This function enables or disables loopback operation for the indicated NORM sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="loopback">If loopbackEnable
        /// is set to true, loopback operation is enabled which allows the application to receive its own message traffic. Thus,
        /// an application which is both actively receiving and sending may receive its own transmissions.</param>
        /// <returns>A return value of true indicates success while a return value of false indicates that the loopback operation could not be set. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetLoopback(long sessionHandle, bool loopback);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetMessageTrace(long sessionHandle, bool flag);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxLoss(long sessionHandle, double precent);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetRxLoss(long sessionHandle, double precent);

        /// <summary>
        /// This function allows NORM debug output to be directed to a file instead of the default STDERR.
        /// </summary>
        /// <param name="instanceHandle">Used to identify application in the NormSession. </param>
        /// <param name="path">Full path and name of the debug log. </param>
        /// <returns>The function returns true on success. If the specified file cannot be opened a value of false is returned. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormOpenDebugLog(long instanceHandle, string path);

        /// <summary>
        /// This function disables NORM debug output to be directed to a file instead of the default STDERR.
        /// </summary>
        /// <param name="instanceHandle">Used to identify application in the NormSession. </param>
        /// <returns>The function returns true on success. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormCloseDebugLog(long instanceHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormOpenDebugPipe(long instanceHandle, string pipeName);

        /// <summary>
        /// This function controls the verbosity of NORM debugging output. Higher values of level result in more detailed
        /// output. The highest level of debugging is 12. The debug output consists of text written to STDOUT by default but
        /// may be directed to a log file using the NormOpenDebugLog() function.
        /// </summary>
        /// <param name="level">
        /// PROTOLIB DEBUG LEVELS:
        /// PL_FATAL=0 The FATAL level designates very severe error events that will presumably lead the application to abort.
        /// PL_ERROR=1 The ERROR level designates error events that might still allow the application to continue running.
        /// PL_WARN=2 The WARN level designates potentially harmful situations.
        /// PL_INFO=3 The INFO level designates informational messages that highlight the progress of the application at coarse-grained level.
        /// PL_DEBUG=4 The DEBUG level designates fine-grained informational events that are most useful to debug an application.
        /// PL_TRACE=5 The TRACE level designates finer-grained informational events than the DEBUG
        /// PL_DETAIL=6 The TRACE level designates even finer-grained informational events than the DEBUG
        /// PL_MAX=7 Turn all comments on
        /// PL_ALWAYS Messages at this level are always printed regardless of debug level
        /// </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDebugLevel(int level);

        /// <summary>
        /// Returns the currently set debug level.
        /// </summary>
        /// <returns>Returns the currently set debug level. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern int NormGetDebugLevel();

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetReportInterval(long sessionHandle, double interval);

        [DllImport(NORM_LIBRARY)]
        public static extern double NormGetReportInterval(long sessionHandle);

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

        /// <summary>
        /// This function terminates the application's participation in a NormSession as a sender. By default, the sender will
        /// immediately exit the session identified by the sessionHandle parameter without notifying the receiver set of its intention.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormStopSender(long sessionHandle);

        /// <summary>
        /// This function sets the transmission rate (in bits per second (bps)) limit used for NormSender transmissions for the given sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="rate">Transmission rate. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxRate(long sessionHandle, double rate);


        /// <summary>
        /// This function retrieves the current sender transmission rate in units of bits per second (bps) for the given sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <returns>This function returns the sender transmission rate in units of bits per second (bps). </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern double NormGetTxRate(long sessionHandle);

        /// <summary>
        /// This function can be used to set a non-default socket buffer size for the UDP socket used by the specified NORM sessionHandle for data transmission.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="bufferSize">The bufferSize parameter specifies the desired socket buffer size in bytes. </param>
        /// <returns>This function returns true upon success and false upon failure. Possible failure modes include an invalid sessionHandle parameter, a call to NormStartReceiver() or NormStartSender() has not yet been made for the
        ///session, or an invalid bufferSize was given. Note some operating systems may require additional system configuration to use non-standard socket buffer sizes. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetTxSocketBuffer(long sessionHandle, long bufferSize);

        /// <summary>
        /// This function controls a scaling factor that is used for sender timer-based flow control for the the specified NORM
        /// sessionHandle. Timer-based flow control works by preventing the NORM sender application from enqueueing
        /// new transmit objects or stream data that would purge "old" objects or stream data when there has been recent
        /// NACK activity for those old objects or data.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="flowControlFactor">The flowControlFactor is used to compute a delay time for when a sender buffered object (or block of stream
        /// data) may be released (i.e. purged) after transmission or applicable NACKs reception. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetFlowControl(long sessionHandle, double flowControlFactor);

        /// <summary>
        /// This function enables (or disables) the NORM sender congestion control operation for the session designated by
        /// the sessionHandle parameter. For best operation, this function should be called before the call to NormStartSender() is made, but congestion control operation can be dynamically enabled/disabled during the course
        /// of sender operation.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="enable">Specifies whether to enable or disable the NORM sender congestion control operation. </param>
        /// <param name="adjustRate">The rate set by NormSetTxRate() has no effect when congestion control operation is enabled, unless the adjustRate
        /// parameter here is set to false. When the adjustRate parameter is set to false, the NORM Congestion Control
        /// operates as usual, with feedback collected from the receiver set and the "current limiting receiver" identified, except
        /// that no actual adjustment is made to the sender's transmission rate. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetCongestionControl(long sessionHandle, bool enable, bool adjustRate);

        /// <summary>
        /// This function sets the range of sender transmission rates within which the NORM congestion control algorithm is
        /// allowed to operate for the given sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="rateMin">rateMin corresponds to the minimum transmission rate (bps). </param>
        /// <param name="rateMax">rateMax corresponds to the maximum transmission rate (bps). </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxRateBounds(long sessionHandle, double rateMin, double rateMax);

        /// <summary>
        /// This function sets limits that define the number and total size of pending transmit objects a NORM sender will allow to be enqueued by the application.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="sizeMax">The sizeMax parameter sets the maximum total size, in bytes, of enqueued objects allowed. </param>
        /// <param name="countMin">The countMin parameter sets the minimum number of objects the application may enqueue, regardless of the objects' sizes and the sizeMax value.</param>
        /// <param name="countMax">countMax parameter sets a ceiling on how many objects may be enqueued, regardless of their total sizes with respect to the sizeMax setting. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetTxCacheBounds(long sessionHandle, long sizeMax, long countMin, long countMax);

        /// <summary>
        /// This function sets the quantity of proactive "auto parity" NORM_DATA messages sent at the end of each FEC coding
        /// block. By default (i.e., autoParity = 0), FEC content is sent only in response to repair requests (NACKs) from receivers.
        /// </summary>
        /// <param name="sesssionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="autoParity">y setting a non-zero value for autoParity, the sender can automatically accompany each coding
        /// block of transport object source data segments ((NORM_DATA messages) with the set number of FEC segments. </param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetAutoParity(long sesssionHandle, short autoParity);

        /// <summary>
        /// This function sets the sender's estimate of group round-trip time (GRTT) (in units of seconds) for the given NORM sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession.  </param>
        /// <param name="grtt"></param>
        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetGrttEstimate(long sessionHandle, double grtt);

        /// <summary>
        /// This function returns the sender's current estimate(in seconds) of group round-trip timing (GRTT) for the given NORM session.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <returns>This function returns the current sender group round-trip timing (GRTT) estimate (in units of seconds). A value
        ///of -1.0 is returned if an invalid session value is provided. </returns>
        [DllImport(NORM_LIBRARY)]
        public static extern double NormGetGrttEstimate(long sessionHandle);

        /// <summary>
        /// This function sets the sender's maximum advertised GRTT value for the given NORM sessionHandle.
        /// </summary>
        /// <param name="sessionHandle">Used to identify application in the NormSession. </param>
        /// <param name="grttMax">The
        /// grttMax parameter, in units of seconds, limits the GRTT used by the group for scaling protocol timers, regardless
        /// of larger measured round trip times. The default maximum for the NRL NORM library is 10 seconds.</param>
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
        public static extern long NormFileEnqueue(long sessionHandle, string fileName, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormDataEnqueue(long sessionHandle, byte[] dataPtr, int dataLen, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormRequeueObject(long sessionHandle, long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormStreamOpen(long sessionHandle, long bufferSize, byte[]? infoPtr, int infoLen);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamClose(long streamHandle, bool graceful);

        [DllImport(NORM_LIBRARY)]
        internal static extern int NormStreamWrite(long streamHandle, byte[] buffer, int numBytes);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamFlush(long streamHandle, bool eom, NormFlushMode flushMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamSetAutoFlush(long streamHandle, NormFlushMode flushMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamSetPushEnable(long streamHandle, bool pushEnable);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStreamHasVacancy(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStreamMarkEom(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetWatermark(long sessionHandle, long objectHandle, bool overrideFlush);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormResetWatermark(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormCancelWatermark(long sessionHandle);

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
        public static extern bool NormStartReceiver(long sessionHandle, long bufferSpace);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormStopReceiver(long sessionHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetRxCacheLimit(long sessionHandle, int countMax);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormSetRxSocketBuffer(long sessionHandle, long bufferSize);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetSilentReceiver(long sessionHandle, bool silent, int maxDelay);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultUnicastNack(long sessionHandle, bool unicastNacks);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetUnicastNack(long remoteSender, bool unicastNacks);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultSyncPolicy(long sessionHandle, NormSyncPolicy syncPolicy);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultNackingMode(long sessionHandle, NormNackingMode nackingMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetNackingMode(long remoteSender, NormNackingMode nackingMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectSetNackingMode(long objectHandle, NormNackingMode nackingMode);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultRepairBoundary(long sessionHandle, NormRepairBoundary repairBoundary);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetRepairBoundary(long remoteSender, NormRepairBoundary repairBoundary);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormSetDefaultRxRobustFactor(long sessionHandle, int robustFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormNodeSetRxRobustFactor(long remoteSender, int robustFactor);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStreamRead(long streamHandle, byte[] buffer, ref int numBytes);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormStreamSeekMsgStart(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormStreamGetReadOffset(long streamHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern NormObjectType NormObjectGetType(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormObjectHasInfo(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetInfoLength(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetInfo(long objectHandle, [Out] byte[] buffer, int bufferLen);

        [DllImport(NORM_LIBRARY)]
        public static extern int NormObjectGetSize(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormObjectGetBytesPending(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectCancel(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectRetain(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern void NormObjectRelease(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormFileGetName(long fileHandle, [Out] char[] nameBuffer, int bufferLen);

        [DllImport(NORM_LIBRARY)]
        public static extern bool NormFileRename(long fileHandle, string fileName);

        [DllImport(NORM_LIBRARY)]
        public static extern nint NormDataAccessData(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern nint NormDataDetachData(long objectHandle);

        [DllImport(NORM_LIBRARY)]
        public static extern long NormObjectGetSender(long objectHandle);

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
    }
}
