using System;
using System.Text;

namespace Mil.Navy.Nrl.Norm
{
    public class NormSession
    {
        private static Dictionary<long, NormSession> _normSessions = new Dictionary<long, NormSession>();
        private long _handle;
        public long LocalNodeId
        {
            get => NormGetLocalNodeId(_handle);
        }

        public double ReportInterval 
        {
            get => NormGetReportInterval(_handle); 
            set => NormSetReportInterval(_handle, value); 
        }

        public double TxRate
        {
            get => NormGetTxRate(_handle);
            set => NormSetTxRate(_handle, value);
        }
        public double GrttEstimate
        {
            get => NormGetGrttEstimate(_handle);
            set => NormSetGrttEstimate(_handle, value);
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
            NormDestroySession(_handle);
        }

        public void SetTxPort(int port)
        {
            SetTxPort(port, false, null);
        }

        public void SetTxPort(int port, bool enableReuse, string? txBindAddress)
        {
            if (!NormSetTxPort(_handle, port, enableReuse, txBindAddress))
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
            NormSetTxOnly(_handle, txOnly, connectToSessionAddress);
        }

        public void SetRxPortReuse(bool enable)
        {
            SetRxPortReuse(enable, null, null, 0);
        }

        public void SetRxPortReuse(bool enable, string? rxBindAddress, string? senderAddress, int senderPort)
        {
            NormSetRxPortReuse(_handle, enable, rxBindAddress, senderAddress, senderPort);
        }

        public void SetEcnSupport(bool ecnEnable, bool ignoreLoss)
        {
            SetEcnSupport(ecnEnable, ignoreLoss, false);
        }

        public void SetEcnSupport(bool ecnEnable, bool ignoreLoss, bool tolerateLoss)
        {
            NormSetEcnSupport(_handle, ecnEnable, ignoreLoss, tolerateLoss);
        }

        public void SetMulticastInterface(string interfaceName)
        {
            if(!NormSetMulticastInterface(_handle, interfaceName))
            {
                throw new IOException("Failed to set multicast interface");
            }
        }

        public void SetSSM(string sourceAddress)
        {
            if(!NormSetSSM(_handle, sourceAddress))
            {
                throw new IOException("Failed to set SSM");
            }
        }

        public void SetTTL(byte ttl)
        {
            if (!NormSetTTL(_handle, ttl))
            {
                throw new IOException("Failed to set TTL");
            }
        }

        public void SetTOS(byte tos)
        {
            if(!NormSetTOS(_handle, tos))
            {
                throw new IOException("Failed to set TOS");
            }
        }

        public void SetLoopback(bool loopbackEnable)
        {
            if (!NormSetLoopback(_handle, loopbackEnable))
            {
                throw new IOException("Failed to set loopback");
            }
        }

        public void SetMessageTrace(bool flag)
        {
            NormSetMessageTrace(_handle, flag);
        }

        public void SetTxLoss(double precent)
        {
            NormSetTxLoss(_handle, precent);
        }

        public void SetRxLoss(double precent)
        {
            NormSetRxLoss(_handle, precent);
        }

        public void SetFlowControl(double precent)
        {
            NormSetFlowControl(_handle, precent);
        }

        public void SetTxSocketBuffer(long bufferSize)
        {
            if(!NormSetTxSocketBuffer(_handle, bufferSize))
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
            NormSetCongestionControl(_handle, enable, adjustRate);
        }

        public void SetTxRateBounds(double rateMin, double rateMax)
        {
            NormSetTxRateBounds(_handle, rateMin, rateMax);
        }

        public void SetTxCacheBounds(long sizeMax, long countMin, long countMax)
        {
            NormSetTxCacheBounds(_handle,sizeMax, countMin, countMax);
        }

        public void StartSender(int sessionId, long bufferSpace, int segmentSize, short blockSize, short numParity, NormFecType fecId)
        {
            if (!NormStartSender(_handle, sessionId, bufferSpace, segmentSize, blockSize, numParity, fecId))
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
            var sessionId = NormGetRandomSessionId();
            StartSender(sessionId, bufferSpace, segmentSize, blockSize, numParity, fecId);
        }

        public void StartSender(long bufferSpace, int segmentSize, short blockSize, short numParity)
        {
            StartSender(bufferSpace, segmentSize, blockSize, numParity, NormFecType.SB);
        }

        public void StopSender()
        {
            NormStopSender(_handle);
        }

        public NormFile FileEnqueue(string filename, byte[] info, int infoOffset, int infoLength)
        {
            byte[]? infoBytes;
            if (info != null)
            {
                infoBytes = info.Skip(infoOffset).Take(infoLength).ToArray();
            }
            else
            {
                infoBytes = null;
                infoLength = 0;
            }
            var objectHandle = NormFileEnqueue(_handle, filename, infoBytes, infoLength);
            if (objectHandle == NORM_OBJECT_INVALID)
            {
                throw new IOException("Failed to enqueue file");
            }
            return new NormFile(objectHandle);
        }

        public NormFile FileEnqueue(string filename, byte[] info, int infoLength)
        {
            return FileEnqueue(filename, info, 0, infoLength);
        }

        public NormFile FileEnqueue(string filename)
        {
            var info = Encoding.ASCII.GetBytes(filename);
            return FileEnqueue(filename, info, info.Length);
        }

        public NormData DataEnqueue(byte[] dataBuffer, int dataOffset, int dataLength, byte[]? info, int infoOffset, int infoLength)
        {
            var dataBytes = dataBuffer.Skip(dataOffset).Take(dataLength).ToArray();
            byte[]? infoBytes;
            if (info != null)
            {
                infoBytes = info.Skip(infoOffset).Take(infoLength).ToArray();
            } 
            else
            {
                infoBytes = null;
                infoLength = 0;
            }
            var objectHandle = NormDataEnqueue(_handle, dataBytes, dataLength, infoBytes, infoLength);
            if (objectHandle == NORM_OBJECT_INVALID)
            {
                throw new IOException("Failed to enqueue data");
            }
            return new NormData(objectHandle);
        }

        public NormData DataEnqueue(byte[] dataBuffer, int dataLength, byte[]? info, int infoLength)
        {
            return DataEnqueue(dataBuffer, 0, dataLength, info, 0, infoLength);
        }

        public NormData DataEnqueue(byte[] dataBuffer, int dataLength)
        {
            return DataEnqueue(dataBuffer, dataLength, null, 0);
        }

        //TODO: Add offsets to StreamOpen
        public NormStream StreamOpen(long bufferSize, byte[]? info, int infoLength)
        {
            var objectHandle = NormStreamOpen(_handle, bufferSize, info, infoLength);
            if (objectHandle == NORM_OBJECT_INVALID)
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
            if(!NormStartReceiver(_handle, bufferSpace))
            {
                throw new IOException("Failed to start receiver");
            }
        }

        public void StopReceiver()
        {
            NormStopReceiver(_handle);
        }

        public void SetAutoParity(short autoParity)
        {
            NormSetAutoParity(_handle, autoParity);
        }

        public void SetGrttMax(double grttMax)
        {
            NormSetGrttMax(_handle, grttMax);
        }

        public void SetGrttProbingMode(NormProbingMode probingMode)
        {
            NormSetGrttProbingMode(_handle, probingMode);
        }

        public void SetGrttProbingInterval(double intervalMin, double intervalMax)
        {
            NormSetGrttProbingInterval(_handle, intervalMin, intervalMax);
        }

        public void SetBackoffFactor(double backoffFactor)
        {
            NormSetBackoffFactor(_handle, backoffFactor);
        }

        public void SetGroupSize(long groupSize)
        {
            NormSetGroupSize(_handle, groupSize);
        }

        public void SetTxRobustFactor(int txRobustFactor)
        {
            NormSetTxRobustFactor(_handle, txRobustFactor);
        }

        public void RequeueObject(NormObject normObject)
        {
            if(!NormRequeueObject(_handle, normObject.Handle))
            {
                throw new IOException("Failed to requeue object");
            }
        }

        public void SetWatermark(NormObject normObject)
        {
            SetWatermark(normObject, false);
        }

        public void SetWatermark(NormObject normObject, bool overrideFlush)
        {
            if(!NormSetWatermark(_handle, normObject.Handle, overrideFlush))
            {
                throw new IOException("Failed to set watermark");
            }
        }

        public void CancelWatermark()
        {
            NormCancelWatermark(_handle);
        }

        public void ResetWatermark()
        {
            if(!NormResetWatermark(_handle))
            {
                throw new IOException("Failed to reset watermark");
            }
        }

        public void AddAckingNode(long nodeId)
        {
            if(!NormAddAckingNode(_handle, nodeId))
            {
                throw new IOException("Failed to add acking node");
            }
        }

        public void RemoveAckingNode(long nodeId)
        {
            NormRemoveAckingNode(_handle, nodeId);
        }

        public NormAckingStatus GetAckingStatus(long nodeId)
        {
            return NormGetAckingStatus(_handle, nodeId);
        }

        public void SendCommand(byte[] cmdBuffer, int cmdLength, bool robust)
        {
            if(!NormSendCommand(_handle, cmdBuffer, cmdLength, robust))
            {
                throw new IOException("Failed to send command");
            }
        }

        public void CancelCommand()
        {
            NormCancelCommand(_handle);
        }

        public void SetRxCacheLimit(int countMax)
        {
            NormSetRxCacheLimit(_handle, countMax);
        }

        public void SetRxSocketBuffer(long bufferSize)
        {
            if(!NormSetRxSocketBuffer(_handle, bufferSize)) 
            {
                throw new IOException("Failed to set rx socket buffer");
            }
        }

        public void SetSilentReceiver(bool silent, int maxDelay)
        {
            NormSetSilentReceiver(_handle, silent, maxDelay);
        }

        public void SetDefaultUnicastNack(bool enable)
        {
            NormSetDefaultUnicastNack(_handle, enable);
        }

        public void SetDefaultSyncPolicy(NormSyncPolicy syncPolicy)
        {
            NormSetDefaultSyncPolicy(_handle, syncPolicy);
        }

        public void SetDefaultNackingMode(NormNackingMode nackingMode)
        {
            NormSetDefaultNackingMode(_handle, nackingMode);
        }

        public void SetDefaultRepairBoundary(NormRepairBoundary repairBoundary)
        {
            NormSetDefaultRepairBoundary(_handle, repairBoundary);
        }

        public void SetDefaultRxRobustFactor(int rxRobustFactor)
        {
            NormSetDefaultRxRobustFactor(_handle, rxRobustFactor);
        }
    }
}