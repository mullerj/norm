namespace Mil.Navy.Nrl.Norm
{
    public class NormNode
    {
        public const long NORM_NODE_ANY = 0xffffffff;
        private long _handle;
        public long Id
        {
            get
            {
                return (long)NormApi.NormNodeGetId(_handle);
            }
        }
        internal NormNode(long handle)
        {
            _handle = handle;
        }
    }
}
