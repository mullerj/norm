using Mil.Navy.Nrl.Norm.Enums;

namespace Mil.Navy.Nrl.Norm
{
    public class NormStream : NormObject
    {
        internal NormStream(long handle) : base(handle)
        {
        }

        public int Write(byte[] buffer, int offset, int length)
        {
            var bytes = buffer.Skip(offset).ToArray();
            return NormApi.NormStreamWrite(_handle, bytes, length);
        }

        public void MarkEom()
        {
            NormApi.NormStreamMarkEom(_handle);
        }

        public void Flush(bool eom, NormFlushMode flushMode)
        {
            NormApi.NormStreamFlush(_handle, eom, flushMode);
        }

        public void Flush()
        {
            Flush(false, NormFlushMode.NORM_FLUSH_PASSIVE);
        }

        public void Close(bool graceful)
        {
            NormApi.NormStreamClose(_handle, graceful);
        }

        public void Close()
        {
            Close(false);
        }
    }
}
