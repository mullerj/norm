using System.Net;
using System.Reflection.Metadata;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace Mil.Navy.Nrl.Norm
{
    public class NormNode
    {
        public const long NORM_NODE_ANY = 0xffffffff;
        private long _handle;

        internal NormNode(long handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// This function retrieves the NormNodeId identifier for the remote participant referenced by the given nodeHandle value. 
        /// The NormNodeId is a 32-bit value used within the NORM protocol to uniquely identify participants within a NORM session.
        /// </summary>
        /// 
        public long Id => NormNodeGetId(_handle);

        /// <summary>
        /// Return the current Endpoint from the current Address
        /// </summary>
        public IPEndPoint Address
        {
            get
            {
                var buffer = new byte[256];
                var bufferLength = buffer.Length;
                if (!NormNodeGetAddress(_handle, buffer, ref bufferLength, out var port))
                {
                    throw new IOException("Failed to get node address");
                }
                buffer = buffer.Take(bufferLength).ToArray();
                var ipAddressText = string.Join('.', buffer);
                var ipAddress = IPAddress.Parse(ipAddressText);

                return new IPEndPoint(ipAddress, port);
            }
        }

        /// <summary>
        /// This function retrieves the advertised estimate of group round-trip timing (GRTT) for the remote sender referenced by the given nodeHandle value.
        /// Newly-starting senders that have been participating as a receiver within a group 
        /// may wish to use this function to provide a more accurate startup estimate of GRTT prior to a call to NormStartSender()
        /// </summary>
        /// <param name="nodeHandle"> This type is used to reference state kept by the NORM implementation with respect to other participants within a NormSession.</param>
        /// <returns>This function returns the remote sender's advertised GRTT estimate in units of seconds. A value of -1.0 is returned upon failure.An invalid nodeHandle parameter value will lead to such failure.</returns>
        public double Grtt => NormNodeGetGrtt(_handle);

        public byte[] Command
        {
            get
            {
                var buffer = new byte[256];
                var bufferLength = buffer.Length;
                if (!NormNodeGetCommand(_handle, buffer, ref bufferLength))
                {
                    throw new IOException("Failed to get command");
                }
                return buffer.Take(bufferLength).ToArray();
            }
        }

        /// <summary>
        /// This function controls the destination address of receiver feedback messages generated in response to a specific
        /// remote NORM sender corresponding to the senderNode parameter.If enable is true, "unicast NACKing" is enabled
        /// while it is disabled for enable equal to false. See the description of NormSetDefaultUnicastNack() for details
        /// on "unicast NACKing" behavior.
        /// </summary>
        /// <param name="state"></param>
        public void SetUnicastNack(bool state)
        {
            NormNodeSetUnicastNack(_handle, state);
        }

        /// <summary>
        /// This function sets the default "nacking mode" used for receiving new objects from a specific sender as identified
        /// by the remoteSender parameter.
        /// </summary>
        /// <param name="remoteSender">Used to specify the remote NORM sender. </param>
        /// <param name="nackingMode">Specifies the nacking mode. </param>
        public void SetNackingMode(NormNackingMode nackingMode)
        {
            NormNodeSetNackingMode(_handle, nackingMode);
        }

        /// <summary>
        /// This function allows the receiver application to customize, for the specific remote sender referenced by the remoteSender parameter, at what points the receiver initiates the NORM NACK repair process during protocol operation.
        /// </summary>
        /// <param name="remoteSender">Used to specify the remote NORM sender. </param>
        /// <param name="repairBoundary">Specifies the repair boundary. </param>
        public void SetRepairBoundary(NormRepairBoundary repairBoundary)
        {
            NormNodeSetRepairBoundary(_handle, repairBoundary);
        }

        /// <summary>
        /// This routine sets the robustFactor as described in NormSetDefaultRxRobustFactor() for an individual remote
        /// sender identified by the remoteSender parameter.
        /// </summary>
        /// <param name="remoteSender">Used to specify the remote NORM sender. </param>
        /// <param name="robustFactor">The robustFactor value determines how
        /// many times a NORM receiver will self-initiate NACKing (repair requests) upon cessation of packet reception from
        /// a sender. The default value is 20. Setting rxRobustFactor to -1 will make the NORM receiver infinitely persistent
        /// (i.e., it will continue to NACK indefinitely as long as it is missing data content).</param>
        public void SetRxRobustFactor(int robustFactor)
        {
            NormNodeSetRxRobustFactor(_handle, robustFactor);
        }

        /// <summary>
        /// This function releases memory resources that were allocated for a remote sender. 
        /// </summary>
        /// <param name="remoteSender">notification for a given remote sender when multiple senders may be providing content</param>
        public void FreeBuffers()
        {
            NormNodeFreeBuffers(_handle);
        }

        /// <summary>
        /// this function allows the application to retain state associated with a given nodeHandle 
        /// value even when the underlying NORM protocol engine might normally 
        /// free the associated state and thus invalidate the NormNodeHandle.
        /// </summary>
        /// <param name="nodeHandle"> This type is used to reference state kept by the NORM implementation with respect to other participants within a NormSession.</param>
        public void Retain()
        {
            NormNodeRetain(_handle);
        }

        /// <summary>
        /// In complement to the NormNodeRetain() function, this API call releases the specified nodeHandle so that the
        /// NORM protocol engine may free associated resources as needed.Once this call is made, the application should
        /// no longer reference the specified NormNodeHandle, unless it is still valid.
        /// </summary>
        /// <param name="nodeHandle"> This type is used to reference state kept by the NORM implementation with respect to other participants within a NormSession.</param>
        public void Release()
        {
            NormNodeRelease(_handle);
        }
    }
}
