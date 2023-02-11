using System.Runtime.InteropServices;

namespace Mil.Navy.Nrl.Norm
{
    public static class Kernel32
    {
        public const string KERNEL32_LIBRARY = "kernel32";
        public const long WAIT_ABANDONED = 0x00000080L;
        public const long WAIT_OBJECT_0 = 0x00000000L;
        public const long WAIT_TIMEOUT = 0x00000102L;
        public const long WAIT_FAILED = 0xFFFFFFFF;

        [DllImport(KERNEL32_LIBRARY)]
        public static extern long WaitForSingleObject(int hHandle, int dwMilliseconds);
    }
}
