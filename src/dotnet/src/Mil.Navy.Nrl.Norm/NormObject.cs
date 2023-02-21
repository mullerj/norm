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

        public long Sender
        {
            get
            {
                var sender = NormApi.NormObjectGetSender(_handle);
                if(sender == NormApi.NORM_NODE_INVALID)
                {
                    throw new IOException("Locally originated sender object");
                }
                return sender;
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

        public void Cancel()
        {
            NormApi.NormObjectCancel(_handle);
        }

        public void Retain()
        {
            NormApi.NormObjectRetain(_handle);
        }

        public void Release()
        {
            NormApi.NormObjectRelease(_handle);
        }

        public override int GetHashCode()
        {
            return (int)Handle;
        }
    }
}
