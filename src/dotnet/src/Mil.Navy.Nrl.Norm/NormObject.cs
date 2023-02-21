using  Mil.Navy.Nrl.Norm.Enums;
namespace Mil.Navy.Nrl.Norm
{
    public class NormObject
    {
        protected long _handle;

        internal NormObject(long handle)
        {
            _handle = handle;
        }

        public long Handle => _handle;

        public byte[]? Info
        {
            get
            {
                if (!NormApi.NormObjectHasInfo(_handle))
                {
                    return null;
                } 
                var length = NormApi.NormObjectGetInfoLength(_handle);
                var buffer = new byte[length];
                NormApi.NormObjectGetInfo(_handle, buffer, length);
                return buffer;
            }
        }

        public NormObjectType Type 
        {
            get 
            {
                return NormApi.NormObjectGetType(_handle);
            }
        }

        public long Size
        {
            get
            {
                return (long)NormApi.NormObjectGetSize(_handle);
            }
        }

        public void SetNackingMode(NormNackingMode nackingMode)
        {
            NormApi.NormObjectSetNackingMode(_handle, nackingMode);
        }

        public long GetBytesPending()
        {
            return NormApi.NormObjectGetBytesPending(_handle);
        }
    }
}
