using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

public static class ArrayExtensions 
{
    public static SafeBuffer ToSafeBuffer(this byte[] buffer)
    {
        using var memoryMappedFile = MemoryMappedFile.CreateNew(null, buffer.Length);
        var memoryAccessor = memoryMappedFile.CreateViewAccessor(0, buffer.Length, MemoryMappedFileAccess.ReadWrite);
        memoryAccessor.WriteArray(0, buffer, 0, buffer.Length);
        return memoryAccessor.SafeMemoryMappedViewHandle;
    }
}