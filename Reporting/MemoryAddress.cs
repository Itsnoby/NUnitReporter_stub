using System;
using System.Runtime.InteropServices;

namespace NUnitReporter.Reporting
{
    public static class MemoryAddress
    {
        public static string Get(object a)
        {
            GCHandle handle = GCHandle.Alloc(a, GCHandleType.Normal);
            try
            {
                IntPtr pointer = GCHandle.ToIntPtr(handle);
                return "0x" + pointer.ToString("X");
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
