using Mil.Navy.Nrl.Norm.Enums;
using System.Text;

namespace Mil.Navy.Nrl.Norm
{
    public class NormSession
    {
        private static Dictionary<long, NormSession> _normSessions = new Dictionary<long, NormSession>();
        private long _handle;

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

        public void SetTxPort(ushort port)
        {
            SetTxPort(port, false, String.Empty);
        }

        public void SetTxPort(ushort port, bool enableReuse, string txBindAddress)
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
            SetRxPortReuse(enable, string.Empty, string.Empty, ushort.MinValue);
        }

        public void SetRxPortReuse(bool enable, string rxBindAddress, string senderAddress, ushort senderPort)
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
    }
}