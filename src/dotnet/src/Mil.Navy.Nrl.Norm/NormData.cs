using System.Runtime.InteropServices;

namespace Mil.Navy.Nrl.Norm
{
    public class NormData : NormObject
    {

        /// <summary>
        /// The data storage area associated with a transport object of type NORM_OBJECT_DATA.
        /// </summary>
        public byte[] Data
        {
            get
            {
                var dataPointer = NormDataAccessData(_handle);
                var length = NormObjectGetSize(_handle);
                var data = new byte[length];
                for (var i = 0; i < length; i++)
                {
                    data[i] = Marshal.ReadByte(dataPointer, i);
                }
                return data;
            }
        }
        internal NormData(long handle) : base(handle)
        {
        }
    }
}
