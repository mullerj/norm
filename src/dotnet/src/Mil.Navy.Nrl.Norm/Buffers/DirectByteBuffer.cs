using System.Runtime.InteropServices;

namespace Mil.Navy.Nrl.Norm.Buffers 
{
    public sealed class DirectByteBuffer : ByteBuffer
    {
        internal DirectByteBuffer(int capacity)
        {
            SetHandle(Marshal.AllocHGlobal(capacity));
            Initialize(Convert.ToUInt64(capacity));
        }
    }
}