using System;
using System.Runtime.InteropServices;

namespace MagikaSharp;

public sealed class MagikaSession : IDisposable
{
    private IntPtr _handle;

    public MagikaSession()
    {
        _handle = NativeMagika.magika_session_new();
        if (_handle == IntPtr.Zero)
            throw new Exception("Failed to allocate Magika session.");
    }

    public string IdentifyFile(string filePath)
    {
        IntPtr strPtr = NativeMagika.magika_identify_file(_handle, filePath);

        if (strPtr == IntPtr.Zero)
            throw new Exception("Magika failed to classify the file.");

        string result = Marshal.PtrToStringAnsi(strPtr)!;

        NativeMagika.magika_string_free(strPtr);

        return result;
    }
    
    public string IdentifyBytes(byte[] data)
    {
        IntPtr strPtr = NativeMagika.magika_identify_bytes(
            _handle,
            data,
            (UIntPtr)data.Length
        );

        if (strPtr == IntPtr.Zero)
            throw new Exception("Magika failed to classify the bytes.");

        string label = Marshal.PtrToStringAnsi(strPtr)!;
        NativeMagika.magika_string_free(strPtr);
        return label;
    }

    public void Dispose()
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMagika.magika_session_free(_handle);
            _handle = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    ~MagikaSession() => Dispose();
}