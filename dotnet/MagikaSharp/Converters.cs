using System;
using System.Runtime.InteropServices;

namespace MagikaSharp;

internal static class Converters
{
    public static MagikaTypeInfo FromNative(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return null;

        var native = Marshal.PtrToStructure<CTypeInfo>(ptr);

        // Convert strings
        string label       = Marshal.PtrToStringAnsi(native.label)!;
        string mime        = Marshal.PtrToStringAnsi(native.mime_type)!;
        string group       = Marshal.PtrToStringAnsi(native.group)!;
        string desc        = Marshal.PtrToStringAnsi(native.description)!;

        // Convert char** extensions
        string[] exts = new string[(int)native.extensions_len];
        for (int i = 0; i < exts.Length; i++)
        {
            IntPtr extPtr = Marshal.ReadIntPtr(native.extensions, i * IntPtr.Size);
            exts[i] = Marshal.PtrToStringAnsi(extPtr)!;
        }

        return new MagikaTypeInfo
        {
            Label = label,
            MimeType = mime,
            Group = group,
            Description = desc,
            Extensions = exts,
            IsText = native.is_text,
        };
    }
}
