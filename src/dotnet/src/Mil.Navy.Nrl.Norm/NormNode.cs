﻿using System.Net;

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
        /// The NormNodeId identifier for the remote participant.
        /// </summary>
        public long Id => NormNodeGetId(_handle);

        /// <summary>
        /// The current network source address detected for packets received from remote NORM sender
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
        /// The advertised estimate of group round-trip timing (GRTT) for the remote sender.
        /// </summary>
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
        /// This function controls the destination address of receiver feedback messages generated in response to a specific remote NORM sender.
        /// </summary>
        /// <param name="state">If state is true, "unicast NACKing" is enabled</param>
        public void SetUnicastNack(bool state)
        {
            NormNodeSetUnicastNack(_handle, state);
        }

        /// <summary>
        /// This function sets the default "nacking mode" used for receiving new objects from a specific sender.
        /// </summary>
        /// <param name="nackingMode">Specifies the nacking mode. </param>
        public void SetNackingMode(NormNackingMode nackingMode)
        {
            NormNodeSetNackingMode(_handle, nackingMode);
        }

        /// <summary>
        /// This function allows the receiver application to customize at what points the receiver initiates the NORM NACK repair process during protocol operation.
        /// </summary>
        /// <param name="repairBoundary">Specifies the repair boundary. </param>
        public void SetRepairBoundary(NormRepairBoundary repairBoundary)
        {
            NormNodeSetRepairBoundary(_handle, repairBoundary);
        }

        /// <summary>
        /// This routine sets the robustFactor as described in NormSetDefaultRxRobustFactor() for an individual remote sender.
        /// </summary>
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
        /// This function allows the application to retain state associated even when the underlying NORM protocol engine might normally free the associated state.
        /// </summary>
        public void Retain()
        {
            NormNodeRetain(_handle);
        }

        /// <summary>
        /// This API call releases the Node so that the NORM protocol engine may free associated resources as needed.
        /// </summary>
        public void Release()
        {
            NormNodeRelease(_handle);
        }
    }
}
