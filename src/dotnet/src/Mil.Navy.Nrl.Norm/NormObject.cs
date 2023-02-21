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
                if (!NormObjectHasInfo(_handle))
                {
                    return null;
                } 
                var length = NormObjectGetInfoLength(_handle);
                var buffer = new byte[length];
                NormObjectGetInfo(_handle, buffer, length);
                return buffer;
            }
        }

        public NormObjectType Type 
        {
            get 
            {
                return NormObjectGetType(_handle);
            }
        }

        public long Size
        {
            get
            {
                return NormObjectGetSize(_handle);
            }
        }

        public long Sender
        {
            get
            {
                var sender = NormObjectGetSender(_handle);
                if(sender == NORM_NODE_INVALID)
                {
                    throw new IOException("Locally originated sender object");
                }
                return sender;
            }
        }

        public void SetNackingMode(NormNackingMode nackingMode)
        {
            NormObjectSetNackingMode(_handle, nackingMode);
        }

        public long GetBytesPending()
        {
            return NormObjectGetBytesPending(_handle);
        }

        public void Cancel()
        {
            NormObjectCancel(_handle);
        }

        public void Retain()
        {
            NormObjectRetain(_handle);
        }

        public void Release()
        {
            NormObjectRelease(_handle);
        }

        public override int GetHashCode()
        {
            return (int)_handle;
        }

        public override bool Equals(object? obj)
        {
            if(obj?.GetType() != GetType())
            {
                return false;
            }
            return _handle == ((NormObject)obj).Handle;
        }
    }
}
