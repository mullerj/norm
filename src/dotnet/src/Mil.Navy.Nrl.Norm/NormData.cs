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
                var dataPointer = NormApi.NormDataAccessData(_handle);
                var length = NormApi.NormObjectGetSize(_handle);
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
