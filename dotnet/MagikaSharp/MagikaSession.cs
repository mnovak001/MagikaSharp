using System;
using System.Runtime.InteropServices;

namespace MagikaSharp;

/// <summary>
/// Provides a safe managed wrapper around the native Magika session.
/// 
/// A <see cref="MagikaSession"/> instance owns an underlying native
/// <c>magika::Session</c> object and must be disposed when no longer needed.
/// </summary>
/// <remarks>
/// Instances of this class are **not thread-safe** unless externally synchronized,
/// because the underlying Magika session object is mutable.
/// 
/// The session object keeps ONNX Runtime and model resources in memory. You should
/// generally reuse a single <see cref="MagikaSession"/> for multiple file analyses
/// rather than creating new ones repeatedly.
/// </remarks>
public sealed class MagikaSession : IDisposable
{
    private IntPtr _ptr;

    /// <summary>
    /// Initializes a new Magika session by constructing a native <c>magika::Session</c>.
    /// </summary>
    /// <exception cref="Exception">Thrown if the native session creation fails.</exception>
    public MagikaSession()
    {
        _ptr = NativeMagika.magika_session_new();
        if (_ptr == IntPtr.Zero)
        {
            throw new Exception("Failed to create Magika session (native pointer is null).");
        }
    }

    /// <summary>
    /// Releases the native Magika session and any associated unmanaged resources.
    /// </summary>
    /// <remarks>
    /// After disposal, the instance becomes unusable.
    /// Calling <see cref="Dispose"/> multiple times is safe.
    /// </remarks>
    public void Dispose()
    {
        if (_ptr != IntPtr.Zero)
        {
            NativeMagika.magika_session_free(_ptr);
            _ptr = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    ~MagikaSession() => Dispose();

    /// <summary>
    /// Identifies a file on disk using the Magika file type detection model.
    /// </summary>
    /// <param name="path">Path to the file to analyze.</param>
    /// <returns>
    /// A <see cref="MagikaTypeInfo"/> object containing the detected file type metadata,
    /// or <c>null</c> if detection fails.
    /// </returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the session has already been disposed.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="path"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// Internally this function calls the native <c>magika_identify_file</c>.
    /// </remarks>
    public MagikaTypeInfo IdentifyFile(string path)
    {
        if (_ptr == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(MagikaSession));

        if (path == null)
            throw new ArgumentNullException(nameof(path));

        IntPtr infoPtr = NativeMagika.magika_identify_file(_ptr, path);

        if (infoPtr == IntPtr.Zero)
            return null;

        var info = Converters.FromNative(infoPtr);
        NativeMagika.magika_typeinfo_free(infoPtr);
        return info;
    }

    /// <summary>
    /// Identifies an in-memory byte buffer using the Magika model.
    /// </summary>
    /// <param name="data">The raw binary data representing a file.</param>
    /// <returns>
    /// A <see cref="MagikaTypeInfo"/> structure describing the content,
    /// or <c>null</c> if detection fails.
    /// </returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the session has already been disposed.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="data"/> is <c>null</c>.
    /// </exception>
    public MagikaTypeInfo IdentifyBytes(byte[] data)
    {
        if (_ptr == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(MagikaSession));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

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
