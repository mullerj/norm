using System.Runtime.InteropServices;

namespace Mil.Navy.Nrl.Norm.Buffers 
{
    public abstract class ByteBuffer : SafeBuffer
    {
        protected ByteBuffer() : base(true)
        {       
        }

        public static ByteBuffer AllocateDirect(int capacity) {
            return new DirectByteBuffer(capacity);
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }
}