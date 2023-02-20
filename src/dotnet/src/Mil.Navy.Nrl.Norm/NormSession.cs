using Mil.Navy.Nrl.Norm.Enums;
using System.Text;

namespace Mil.Navy.Nrl.Norm
{
    public class NormSession
    {
        private static Dictionary<long, NormSession> _normSessions = new Dictionary<long, NormSession>();
        private long _handle;
        public long LocalNodeId
        {
            get { return NormApi.NormGetLocalNodeId(_handle); }
        }
        public double ReportInterval { get; set; }
        public double TxRate
        {
            get
            {
                return NormApi.NormGetTxRate(_handle);
            }
            set
            {
                NormApi.NormSetTxRate(_handle, value);
            }
        }
        public double GrttEstimate
        {
            get
            {
                return NormApi.NormGetGrttEstimate(_handle);
            }
            set
            {
                NormApi.NormSetGrttEstimate(_handle, value);
            }
        }
        
        internal NormSession(long handle)
        {
            _handle = handle;
            _normSessions.Add(handle, this);
        }

        internal static NormSession GetSession(long handle)
        {
            return _normSessions[handle];
        }

        public void DestroySession()
        {
            _normSessions.Remove(_handle);
            DestroySessionNative();
        }

        private void DestroySessionNative()
        {
            NormApi.NormDestroySession(_handle);
        }

        public void SetTxPort(int port)
        {
            SetTxPort(port, false, null);
        }

        public void SetTxPort(int port, bool enableReuse, string? txBindAddress)
        {
            if (!NormApi.NormSetTxPort(_handle, port, enableReuse, txBindAddress))
            {
                throw new IOException("Failed to set Tx Port");
            }
        }

        public void SetTxOnly(bool txOnly)
        {
            SetTxOnly(txOnly, false);
        }

        public void SetTxOnly(bool txOnly, bool connectToSessionAddress)
        {
            NormApi.NormSetTxOnly(_handle, txOnly, connectToSessionAddress);
        }

        public void SetRxPortReuse(bool enable)
        {
            SetRxPortReuse(enable, null, null, 0);
        }

        public void SetRxPortReuse(bool enable, string? rxBindAddress, string? senderAddress, int senderPort)
        {
            NormApi.NormSetRxPortReuse(_handle, enable, rxBindAddress, senderAddress, senderPort);
        }

        public void SetMulticastInterface(string interfaceName)
        {
            if(!NormApi.NormSetMulticastInterface(_handle, interfaceName))
            {
                throw new IOException("Failed to set multicast interface");
            }
        }

        public void SetSSM(string sourceAddress)
        {
            if(!NormApi.NormSetSSM(_handle, sourceAddress))
            {
                throw new IOException("Failed to set SSM");
            }
        }

        public void SetTTL(byte ttl)
        {
            if (!NormApi.NormSetTTL(_handle, ttl))
            {
                throw new IOException("Failed to set TTL");
            }
        }

        public void SetTOS(byte tos)
        {
            if(!NormApi.NormSetTOS(_handle, tos))
            {
                throw new IOException("Failed to set TOS");
            }
        }

        public void SetLoopback(bool loopbackEnable)
        {
            if (!NormApi.NormSetLoopback(_handle, loopbackEnable))
            {
                throw new IOException("Failed to set loopback");
            }
        }

        public void SetMessageTrace(bool flag)
        {
            NormApi.NormSetMessageTrace(_handle, flag);
        }

        public void SetTxLoss(double precent)
        {
            NormApi.NormSetTxLoss(_handle, precent);
        }

        public void SetRxLoss(double precent)
        {
            NormApi.NormSetRxLoss(_handle, precent);
        }

        public void SetFlowControl(double precent)
        {
            NormApi.NormSetFlowControl(_handle, precent);
        }

        public void SetTxSocketBuffer(long bufferSize)
        {
            if(!NormApi.NormSetTxSocketBuffer(_handle, bufferSize))
            {
                throw new IOException("Failed to set tx socket buffer");
            }
        }

        public void SetCongestionControl(bool enable)
        {
            SetCongestionControl(enable, true);
        }

        public void SetCongestionControl(bool enable, bool adjustRate)
        {
            NormApi.NormSetCongestionControl(_handle, enable, adjustRate);
        }

        public void SetTxRateBounds(double rateMin, double rateMax)
        {
            NormApi.NormSetTxRateBounds(_handle, rateMin, rateMax);
        }

        public void SetTxCacheBounds(long sizeMax, long countMin, long countMax)
        {
            NormApi.NormSetTxCacheBounds(_handle,sizeMax, countMin, countMax);
        }

        public void StartSender(int sessionId, long bufferSpace, int segmentSize, short blockSize, short numParity, NormFecType fecId)
        {
            if (!NormApi.NormStartSender(_handle, sessionId, bufferSpace, segmentSize, blockSize, numParity, fecId))
            {
                throw new IOException("Failed to start sender");
            }
        }

        public void StartSender(int sessionId, long bufferSpace, int segmentSize, short blockSize, short numParity)
        {
            StartSender(sessionId, bufferSpace, segmentSize, blockSize, numParity, NormFecType.SB);
        }

        public void StartSender(long bufferSpace, int segmentSize, short blockSize, short numParity, NormFecType fecId)
        {
            var sessionId = NormApi.NormGetRandomSessionId();
            StartSender(sessionId, bufferSpace, segmentSize, blockSize, numParity, fecId);
        }

        public void StartSender(long bufferSpace, int segmentSize, short blockSize, short numParity)
        {
            StartSender(bufferSpace, segmentSize, blockSize, numParity, NormFecType.SB);
        }

        public void StopSender()
        {
            NormApi.NormStopSender(_handle);
        }

        public NormFile FileEnqueue(string filename, byte[] info, int infoLength)
        {
            var objectHandle = NormApi.NormFileEnqueue(_handle, filename, info, infoLength);
            if (objectHandle == NormApi.NORM_OBJECT_INVALID)
            {
                throw new IOException("Failed to enqueue file");
            }
            return new NormFile(objectHandle);
        }

        public NormFile FileEnqueue(string filename)
        {
            var info = Encoding.ASCII.GetBytes(filename);
            return FileEnqueue(filename, info, info.Length);
        }

        public NormData DataEnqueue(byte[] dataBuffer, int dataOffset, int dataLength, byte[] info, int infoOffset, int infoLength)
        {
            var objectHadle = NormApi.NormDataEnqueue(_handle, dataBuffer, dataLength, info, infoLength);
            if(objectHadle == NormApi.NORM_OBJECT_INVALID)
            {
                throw new IOException("Failed to enqueue data");
            }
            return new NormData(objectHadle);
        }

        public NormData DataEnqueue(byte[] dataBuffer, int dataOffset, int dataLength)
        {
            return DataEnqueue(dataBuffer, dataOffset, dataLength, null, 0, 0);
        }

        public NormStream StreamOpen(long bufferSize, byte[]? info, int infoLength)
        {
            var objectHandle = NormApi.NormStreamOpen(_handle, bufferSize, info, infoLength);
            if (objectHandle == NormApi.NORM_OBJECT_INVALID)
            {
                throw new IOException("Failed to open stream");
            }
            return new NormStream(objectHandle);
        }

        public NormStream StreamOpen(long bufferSize)
        {
            return StreamOpen(bufferSize, null, 0);
        }

        public void StartReceiver(long bufferSpace)
        {
            if(!NormApi.NormStartReceiver(_handle, bufferSpace))
            {
                throw new IOException("Failed to start receiver");
            }
        }

        public void StopReceiver()
        {
            NormApi.NormStopReceiver(_handle);
        }

        public void SetAutoParity(short autoParity)
        {
            NormApi.NormSetAutoParity(_handle, autoParity);
        }

        public void SetGrttMax(double grttMax)
        {
            NormApi.NormSetGrttMax(_handle, grttMax);
        }

        public void SetGrttProbingMode(NormProbingMode probingMode)
        {
            NormApi.NormSetGrttProbingMode(_handle, probingMode);
        }

        public void SetGrttProbingInterval(double intervalMin, double intervalMax)
        {
            NormApi.NormSetGrttProbingInterval(_handle, intervalMin, intervalMax);
        }

        public void SetBackoffFactor(double backoffFactor)
        {
            NormApi.NormSetBackoffFactor(_handle, backoffFactor);
        }

        public void SetGroupSize(long groupSize)
        {
            NormApi.NormSetGroupSize(_handle, groupSize);
        }

        public void SetTxRobustFactor(int txRobustFactor)
        {
            NormApi.NormSetTxRobustFactor(_handle, txRobustFactor);
        }

        public void RequeueObject(NormObject normObject)
        {
            if(!NormApi.NormRequeueObject(_handle, normObject.Handle))
            {
                throw new IOException("Failed to requeue object");
            }
        }

        public void SetWatermark(NormObject normObject)
        {
            SetWatermark(normObject, true);
        }

        public void SetWatermark(NormObject normObject, bool overrideFlush)
        {
            if(!NormApi.NormSetWatermark(_handle, normObject.Handle, overrideFlush))
            {
                throw new IOException("Failed to set watermark");
            }
        }

        public void CancelWatermark()
        {
            NormApi.NormCancelWatermark(_handle);
        }

        public void ResetWatermark()
        {
            if(!NormApi.NormResetWatermark(_handle))
            {
                throw new IOException("Failed to reset watermark");
            }
        }

        public void AddAckingNode(long nodeId)
        {
            if(!NormApi.NormAddAckingNode(_handle, nodeId))
            {
                throw new IOException("Failed to add acking node");
            }
        }

        public void RemoveAckingNode(long nodeId)
        {
            NormApi.NormRemoveAckingNode(_handle, nodeId);
        }

        public void SendCommand(byte[] cmdBuffer, int cmdLength)
        {
            SendCommand(cmdBuffer, cmdLength, false);
        }

        public void SendCommand(byte[] cmdBuffer, int cmdLength, bool robust)
        {
            if(!NormApi.NormSendCommand(_handle, cmdBuffer, cmdLength, robust))
            {
                throw new IOException("Failed to send command");
            }
        }

        public void CancelCommand()
        {
            NormApi.NormCancelCommand(_handle);
        }

        public void SetRxCacheLimit(int countMax)
        {
            NormApi.NormSetRxCacheLimit(_handle, countMax);
        }

        public void SetRxSocketBuffer(long bufferSize)
        {
            if(!NormApi.NormSetRxSocketBuffer(_handle, bufferSize)) 
            {
                throw new IOException("Failed to set rx socket buffer");
            }
        }

        public void SetSilentReceiver(bool silent)
        {
            SetSilentReceiver(silent, -1);
        }

        public void SetSilentReceiver(bool silent, int maxDelay)
        {
            NormApi.NormSetSilentReceiver(_handle, silent, maxDelay);
        }

        public void SetDefaultUnicastNack(bool enable)
        {
            NormApi.NormSetDefaultUnicastNack(_handle, enable);
        }

        public void SetDefaultSyncPolicy(NormSyncPolicy syncPolicy)
        {
            NormApi.NormSetDefaultSyncPolicy(_handle, syncPolicy);
        }

        public void SetDefaultNackingMode(NormNackingMode nackingMode)
        {
            NormApi.NormSetDefaultNackingMode(_handle, nackingMode);
        }

        public void SetDefaultRepairBoundary(NormRepairBoundary repairBoundary)
        {
            NormApi.NormSetDefaultRepairBoundary(_handle, repairBoundary);
        }

        public void SetDefaultRxRobustFactor(int rxRobustFactor)
        {
            NormApi.NormSetDefaultRxRobustFactor(_handle, rxRobustFactor);
        }
    }
}