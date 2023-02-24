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
            var bytes = buffer.Skip(offset).Take(length).ToArray();
            return NormStreamWrite(_handle, bytes, length);
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
