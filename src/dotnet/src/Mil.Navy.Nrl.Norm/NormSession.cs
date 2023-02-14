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

        public void SetLoopback(bool loopbackEnable)
        {
            if (!NormApi.NormSetLoopback(_handle, loopbackEnable))
            {
                throw new IOException("Failed to set loopback");
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
            var dataPtr = Encoding.ASCII.GetString(dataBuffer.ToArray());
            var objectHadle = NormApi.NormDataEnqueue(_handle, dataPtr, dataLength, info, infoLength);
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