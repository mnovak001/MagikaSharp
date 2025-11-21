using System;
using System.Runtime.InteropServices;

namespace MagikaSharp;

internal static class NativeMagika
{
    private const string Lib = "libmagika_ffi"; 
    // Linux: libmagika_ffi.so
    // Windows: magika_ffi.dll
    // macOS: libmagika_ffi.dylib

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr magika_session_new();

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void magika_session_free(IntPtr session);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr magika_identify_file(
        IntPtr session,
        [MarshalAs(UnmanagedType.LPStr)] string path);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void magika_string_free(IntPtr strPtr);
    
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void magika_typeinfo_free(IntPtr infoPtr);
    
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr magika_identify_bytes(
        IntPtr session,
        byte[] data,
        UIntPtr len
    );
}