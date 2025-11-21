using System;
using System.Runtime.InteropServices;

namespace MagikaSharp;

[StructLayout(LayoutKind.Sequential)]
internal struct CTypeInfo
{
    public IntPtr label;          // char*
    public IntPtr mime_type;      // char*
    public IntPtr group;          // char*
    public IntPtr description;    // char*

    public IntPtr extensions;     // char**
    public nuint extensions_len;  // usize
    
    [MarshalAs(UnmanagedType.I1)]
    public bool is_text;
}