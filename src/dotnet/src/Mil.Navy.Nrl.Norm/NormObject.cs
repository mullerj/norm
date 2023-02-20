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
    }
}
