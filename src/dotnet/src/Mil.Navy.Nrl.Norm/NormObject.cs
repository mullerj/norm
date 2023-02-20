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
    }
}
