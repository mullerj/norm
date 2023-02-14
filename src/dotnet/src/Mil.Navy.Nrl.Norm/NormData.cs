using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mil.Navy.Nrl.Norm
{
    public class NormData : NormObject
    {
        public byte[] Data
        {
            get
            {
                var buffer = NormApi.NormDataAccessData(_handle);
                var length = NormApi.NormObjectGetSize(_handle);
                var result = Marshal.PtrToStringAnsi(buffer);
                return Encoding.ASCII.GetBytes(result);
            }
        }
        internal NormData(long handle) : base(handle)
        {
        }
    }
}
