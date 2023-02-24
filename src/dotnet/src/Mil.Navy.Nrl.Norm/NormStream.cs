﻿using System.Runtime.InteropServices;
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
            byte[] managedArray = new byte[length];
            Marshal.Copy(newPtr, managedArray, 0, length);
            return NormStreamWrite(_handle, managedArray, length);
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

        public int Read(byte[] buffer, int offset, int length)
        {
            var bytes = new byte[length];

            if (!NormStreamRead(_handle, bytes, ref length))
            {
                return -1;
            }

            for (var i = 0; i < length; i++)
            {
                var bufferPosition = offset + i;
                if (bufferPosition < buffer.Length)
                {
                    buffer[bufferPosition] = bytes[i];
                } 
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
