namespace Mil.Navy.Nrl.Norm
{
    public class NormStream : NormObject
    {
        public bool HasVacancy
        {
            get
            {
                return NormApi.NormStreamHasVacancy(_handle);
            }
        }

        public long ReadOffset
        {
            get
            {
                return NormApi.NormStreamGetReadOffset(_handle);
            }
        }

        internal NormStream(long handle) : base(handle)
        {
        }

        public int Write(byte[] buffer, int length)
        {
            return NormStreamWrite(_handle, buffer, length);
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
    }
}
