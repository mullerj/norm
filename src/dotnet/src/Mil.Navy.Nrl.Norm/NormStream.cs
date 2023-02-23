using System.Runtime.InteropServices;
namespace Mil.Navy.Nrl.Norm
{
    public class NormStream : NormObject
    {
        public bool HasVacancy
        {
            get
            {
                return NormStreamHasVacancy(_handle);
            }
        }

        public long ReadOffset
        {
            get
            {
                return NormStreamGetReadOffset(_handle);
            }
        }

        internal NormStream(long handle) : base(handle)
        {
        }

        public int Write(byte[] buffer, int offset, int length)
        {
            IntPtr ptr = IntPtr.Zero;
            if(buffer != null && buffer.Length > 0)
            {
                ptr = Marshal.AllocHGlobal(buffer.Length);
                Marshal.Copy(buffer, 0, ptr, buffer.Length);
            }
            IntPtr newPtr = IntPtr.Add(ptr, offset);
            var size = length - offset;
            byte[] managedArray = new byte[size];
            Marshal.Copy(newPtr, managedArray, 0, size);
            return NormStreamWrite(_handle, managedArray, size);
        }

        public void MarkEom()
        {
            NormStreamMarkEom(_handle);
        }

        public void Flush(bool eom, NormFlushMode flushMode)
        {
            NormStreamFlush(_handle, eom, flushMode);
        }

        public void Flush()
        {
            Flush(false, NormFlushMode.NORM_FLUSH_PASSIVE);
        }

        public void Close(bool graceful)
        {
            NormStreamClose(_handle, graceful);
        }

        public void Close()
        {
            Close(false);
        }

        public int Read(byte[] buffer, int length)
        {
            if (!NormStreamRead(_handle, buffer, ref length))
            {
                return -1;
            }
            return length;
        }

        public bool SeekMsgStart()
        {
            return NormStreamSeekMsgStart(_handle);
        }

        public void SetPushEnable(bool pushEnable)
        {
            NormStreamSetPushEnable(_handle, pushEnable);
        }

        public void SetAutoFlush(NormFlushMode flushMode)
        {
            NormStreamSetAutoFlush(_handle, flushMode);
        }
    }
}
