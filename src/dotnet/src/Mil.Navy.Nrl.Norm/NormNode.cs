using System.Net;

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

        public long Id => NormNodeGetId(_handle);

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

        public void SetUnicastNack(bool state)
        {
            NormNodeSetUnicastNack(_handle, state);
        }

        public void SetNackingMode(NormNackingMode nackingMode)
        {
            NormNodeSetNackingMode(_handle, nackingMode);
        }

        public void SetRepairBoundary(NormRepairBoundary repairBoundary)
        {
            NormNodeSetRepairBoundary(_handle, repairBoundary);
        }

        public void SetRxRobustFactor(int robustFactor)
        {
            NormNodeSetRxRobustFactor(_handle, robustFactor);
        }

        public void FreeBuffers()
        {
            NormNodeFreeBuffers(_handle);
        }

        public void Retain()
        {
            NormNodeRetain(_handle);
        }

        public void Release()
        {
            NormNodeRelease(_handle);
        }
    }
}
