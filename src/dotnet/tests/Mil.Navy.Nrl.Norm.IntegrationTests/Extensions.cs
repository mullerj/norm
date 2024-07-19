using System.Runtime.InteropServices;
using Mil.Navy.Nrl.Norm.Buffers;

namespace Mil.Navy.Nrl.Norm.IntegrationTests
{
    /// <summary>
    /// Extension methods to assist with testing
    /// </summary>
    internal static class Extensions 
    {
        /// <summary>
        /// Obtain a safe buffer from a byte array buffer
        /// </summary>
        /// <param name="buffer">The byte array buffer</param>
        /// <returns>The safe buffer based on the byte array</returns>
        internal static SafeBuffer ToSafeBuffer(this byte[] buffer)
        {
            var safeBuffer = ByteBuffer.AllocateDirect(buffer.Length);
            safeBuffer.WriteArray(0, buffer, 0, buffer.Length);
            return safeBuffer;
        }
    }
}

