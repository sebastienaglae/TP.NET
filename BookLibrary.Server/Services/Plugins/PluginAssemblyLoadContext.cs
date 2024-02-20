using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace BookLibrary.Server.Services.Plugins;

public class PluginAssemblyLoadContext : AssemblyLoadContext, IDisposable
{
    private readonly HashSet<IntPtr> _loadedLibraries = [];
    
    public PluginAssemblyLoadContext(string name, bool isCollectible = true) : base(name, isCollectible)
    {
    }

    public IntPtr LoadUnmanagedLibraryFromPath(string libraryPath)
    {
        IntPtr libraryHandle = LoadUnmanagedDllFromPath(libraryPath);
        if (libraryHandle != IntPtr.Zero)
            _loadedLibraries.Add(libraryHandle);
        
        return libraryHandle;
    }
    
    public void Dispose()
    {
        foreach (var library in _loadedLibraries)
            NativeLibrary.Free(library);
        
        _loadedLibraries.Clear();
    }
}