using System;
using System.Runtime.InteropServices;

namespace MagikaSharp;

public sealed class MagikaSession : IDisposable
{
    private IntPtr _ptr;

    public MagikaSession()
    {
        _ptr = NativeMagika.magika_session_new();
        if (_ptr == IntPtr.Zero)
            throw new Exception("Failed to create Magika session");
    }

    public void Dispose()
    {
        if (_ptr != IntPtr.Zero)
        {
            NativeMagika.magika_session_free(_ptr);
            _ptr = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }

    ~MagikaSession() => Dispose();

    // Identify a file
    public MagikaTypeInfo IdentifyFile(string path)
    {
        IntPtr infoPtr = NativeMagika.magika_identify_file(_ptr, path);
        if (infoPtr == IntPtr.Zero)
            return null;

        var info = Converters.FromNative(infoPtr);
        NativeMagika.magika_typeinfo_free(infoPtr);
        return info;
    }

    // Identify bytes
    public MagikaTypeInfo IdentifyBytes(byte[] data)
    {
        IntPtr infoPtr = NativeMagika.magika_identify_bytes(
            _ptr,
            data,
            (UIntPtr)data.Length
        );

        if (infoPtr == IntPtr.Zero)
            return null;

        var info = Converters.FromNative(infoPtr);
        NativeMagika.magika_typeinfo_free(infoPtr);
        return info;
    }
}
