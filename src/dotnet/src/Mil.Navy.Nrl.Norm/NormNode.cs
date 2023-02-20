using Mil.Navy.Nrl.Norm.Enums;
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

        public long Id => NormApi.NormNodeGetId(_handle);

        public IPEndPoint Address
        {
            get
            {
                var buffer = new byte[256];
                var bufferLength = buffer.Length;
                if (!NormApi.NormNodeGetAddress(_handle, buffer, ref bufferLength, out var port))
                {
                    throw new IOException("Failed to get node address");
                }
                buffer = buffer.Take(bufferLength).ToArray();
                var ipAddressText = string.Join('.', buffer);
                var ipAddress = IPAddress.Parse(ipAddressText);

                return new IPEndPoint(ipAddress, port);
            }
        }

        public double Grtt => NormApi.NormNodeGetGrtt(_handle);

        public void SetUnicastNack(bool state)
        {
            NormApi.NormNodeSetUnicastNack(_handle, state);
        }

        public void SetNackingMode(NormNackingMode nackingMode)
        {
            NormApi.NormNodeSetNackingMode(_handle, nackingMode);
        }
    }
}
