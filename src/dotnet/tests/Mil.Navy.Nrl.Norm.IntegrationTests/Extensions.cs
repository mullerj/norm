using System.IO.MemoryMappedFiles;
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
        /// Obtain a byte buffer from a byte array buffer
        /// </summary>
        /// <param name="buffer">The byte array buffer</param>
        /// <returns>The byte buffer based on the byte array</returns>
        internal static ByteBuffer ToByteBuffer(this byte[] buffer)
        {
            var byteBuffer = ByteBuffer.AllocateDirect(buffer.Length);
            byteBuffer.WriteArray(0, buffer, 0, buffer.Length);
            return byteBuffer;
        }

        /// <summary>
        /// Obtain a safe buffer from a byte array buffer
        /// </summary>
        /// <param name="buffer">The byte array buffer</param>
        /// <returns>The safe buffer based on the byte array</returns>
        internal static SafeBuffer ToSafeBuffer(this byte[] buffer)
        {
            using var memoryMappedFile = MemoryMappedFile.CreateNew(null, buffer.Length);
            var memoryAccessor = memoryMappedFile.CreateViewAccessor(0, buffer.Length, MemoryMappedFileAccess.ReadWrite);
            memoryAccessor.WriteArray(0, buffer, 0, buffer.Length);
            return memoryAccessor.SafeMemoryMappedViewHandle;
        }
    }
}

