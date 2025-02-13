using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BookLibrary.ServerPluginKit;

namespace BookLibrary.Server.Services.Plugins;

public class PluginNative : IPluginInterface
{
    private readonly string _name;
    private readonly IntPtr _nativeHandle;
    private GCHandle _gcHandle;
    private IPluginInterface.HttpMap _map;
    
    public PluginNative(string filename, IntPtr nativeHandle)
    {
        _name = filename;
        _nativeHandle = nativeHandle;
        _gcHandle = GCHandle.Alloc(this);
    }
    
    public unsafe void Setup(IPluginInterface.HttpMap map)
    {
        _map = map;
        try
        {
            delegate* unmanaged<IntPtr, char *, char *, delegate* unmanaged<UnmanagedHttpRequest*, UnmanagedHttpResponse*, int>, void> httpMap = &HttpMap;
            if (NativeLibrary.TryGetExport(_nativeHandle, "CreatePlugin", out var createPluginAddress) == false)
                throw new InvalidOperationException($"Plugin {_name} does not have a CreatePlugin method");

            delegate* unmanaged<IntPtr, delegate* unmanaged<IntPtr, char*, char*, delegate* unmanaged<UnmanagedHttpRequest*, UnmanagedHttpResponse*, int>, void>, IntPtr>
                createPlugin =
                    (delegate* unmanaged<IntPtr, delegate* unmanaged<IntPtr, char*, char*, delegate* unmanaged<UnmanagedHttpRequest*, UnmanagedHttpResponse*, int>,
                        void>, IntPtr>) createPluginAddress;
            
            var result = createPlugin(GCHandle.ToIntPtr(_gcHandle), httpMap);
            if (result != IntPtr.Zero)
                throw new InvalidOperationException($"Plugin {_name} failed to initialize with error code {result}");
        }
        finally
        {
            _map = null;
        }
    }

    public unsafe void Dispose()
    {
        try
        {
            if (NativeLibrary.TryGetExport(_nativeHandle, "DestroyPlugin", out var destroyPluginAddress))
            {
                delegate* unmanaged<IntPtr, void> destroyPlugin = (delegate* unmanaged<IntPtr, void>) destroyPluginAddress;
                destroyPlugin(_nativeHandle);
            }
        }
        catch (EntryPointNotFoundException)
        {
            // ignore as the method is optional
        }

        _gcHandle.Free();
    }
    
    [UnmanagedCallersOnly]
    private static unsafe void HttpMap(IntPtr handle, char *method, char *path, delegate* unmanaged<UnmanagedHttpRequest*, UnmanagedHttpResponse*, int> handler)
    {
        // create a managed handler that can be called from the unmanaged world
        var managedHandler = new Func<PluginHttpRequest, PluginHttpResponse>(request =>
        {
            Span<byte> methodSpan = stackalloc byte[Encoding.UTF8.GetByteCount(request.Method)];
            Span<byte> pathSpan = stackalloc byte[Encoding.UTF8.GetByteCount(request.Path)];
            Encoding.UTF8.GetBytes(request.Method, methodSpan);
            Encoding.UTF8.GetBytes(request.Path, pathSpan);
            fixed (byte* methodPtr = methodSpan)
            fixed (byte* pathPtr = pathSpan)
            fixed (byte* bodyPtr = request.Body)
            {
                var unmanagedRequest = new UnmanagedHttpRequest
                {
                    Method = new ByteArray(methodPtr, methodSpan.Length),
                    Path = new ByteArray(pathPtr, pathSpan.Length),
                    Body = new ByteArray(bodyPtr, request.Body.Length)
                };
                var unmanagedResponse = default(UnmanagedHttpResponse);
                var success = handler(&unmanagedRequest, &unmanagedResponse);
                try
                {
                    if (success != 0)
                        throw new InvalidOperationException($"Plugin {handle} failed to handle request {request.Method} {request.Path}");
                    
                    var managedResponse = new PluginHttpResponse
                    {
                        StatusCode = unmanagedResponse.StatusCode,
                        Body = new Span<byte>(unmanagedResponse.Body.Data, unmanagedResponse.Body.Length).ToArray(),
                        Headers = new Dictionary<string, string>(unmanagedResponse.HeaderCount)
                    };
                    for (int i = 0; i < unmanagedResponse.HeaderCount; i++)
                    {
                        var header = unmanagedResponse.Headers[i];
                        managedResponse.Headers.Add(
                            Encoding.UTF8.GetString(header.Key.Data, header.Key.Length),
                            Encoding.UTF8.GetString(header.Value.Data, header.Value.Length)
                        );
                    }
                    return managedResponse;
                }
                finally
                {
                    if (unmanagedResponse.Headers != null)
                    {
                        for (int i = 0; i < unmanagedResponse.HeaderCount; i++)
                        {
                            var header = unmanagedResponse.Headers[i];
                            Marshal.FreeHGlobal((IntPtr) header.Key.Data);
                            Marshal.FreeHGlobal((IntPtr) header.Value.Data);
                        }
                        Marshal.FreeHGlobal((IntPtr) unmanagedResponse.Headers);
                    }
                    Marshal.FreeHGlobal((IntPtr) unmanagedResponse.Body.Data);
                }
            }
        });
        
        // call the map
        GCHandle gcHandle = GCHandle.FromIntPtr(handle);
        PluginNative plugin = (PluginNative) gcHandle.Target;
        if (plugin == null)
            return;
        
        string methodStr = Marshal.PtrToStringUTF8((IntPtr) method);
        string pathStr = Marshal.PtrToStringUTF8((IntPtr) path);
        plugin._map(methodStr!, pathStr!, managedHandler);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct UnmanagedHttpRequest
    {
        public ByteArray Method;
        public ByteArray Path;
        public ByteArray Body;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct UnmanagedHttpResponse
    {
        public int StatusCode;
        public ByteArray Body;
        public UnmanagedHeader* Headers;
        public int HeaderCount;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct UnmanagedHeader
    {
        public ByteArray Key;
        public ByteArray Value;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct ByteArray
    {
        public byte* Data;
        public int Length;
        
        public ByteArray(byte* data, int length)
        {
            Data = data;
            Length = length;
        }
    }
}