using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using BookLibrary.Server.Options;
using BookLibrary.ServerPluginKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Server.Services.Plugins;

public class PluginService : IDisposable
{
    private readonly PluginOptions _options;
    private readonly ILogger<PluginService> _logger;
    private readonly List<string> _pluginPaths = [];
    private readonly List<PluginInfo> _plugins = [];
    
    public PluginService(ILogger<PluginService> logger, IConfiguration configuration)
    {
        _options = configuration.GetSection("Plugins").Get<PluginOptions>() ?? new PluginOptions();
        _logger = logger;
        ScanPlugins();
    }

    public void MapPlugins(IEndpointRouteBuilder builder)
    {
        IPluginInterface.HttpMap httpMap = (method, path, handler) =>
        {
            builder.MapMethods(path, new[] { method }, async context =>
            {
                var bodyArray = Array.Empty<byte>();
                if (context.Request.Method is "POST" or "PUT")
                {
                    var buffer = new byte[4096];
                    List<byte> bodyList = new List<byte>(4096);
                    while (true)
                    {
                        var read = await context.Request.Body.ReadAsync(buffer);
                        if (read == 0)
                            break;
                        bodyList.AddRange(buffer.AsSpan(0, read));
                    }
                    bodyArray = bodyList.ToArray();
                }
                
                var request = new PluginHttpRequest
                {
                    Method = method,
                    Path = path,
                    Body = bodyArray
                };
                var response = handler(request);
                context.Response.StatusCode = response.StatusCode;
                foreach (var (key, value) in response.Headers)
                {
                    context.Response.Headers[key] = value;
                }
                await context.Response.Body.WriteAsync(response.Body);
            });
        };
        
        foreach (string pluginPath in _pluginPaths)
        {
            var (pluginInterface, pluginAssemblyContext) = LoadPlugin(pluginPath);
            if (pluginInterface != null)
            {
                _plugins.Add(new PluginInfo(pluginInterface, pluginAssemblyContext));
                
                pluginInterface.Setup(httpMap);
            }
        }
    }

    private void ScanPlugins()
    {
        if (!Directory.Exists(_options.Directory))
            return;
        
        _logger.LogInformation("Scanning for plugins...");
        foreach (string file in Directory.EnumerateFiles(_options.Directory, "*.dll", SearchOption.AllDirectories))
        {
            _pluginPaths.Add(file);
            _logger.LogInformation("Found plugin: {PluginPath}", file);
        }
        foreach (string file in Directory.EnumerateFiles(_options.Directory, $"*{PluginExtension}", SearchOption.AllDirectories))
        {
            _pluginPaths.Add(file);
            _logger.LogInformation("Found plugin: {PluginPath}", file);
        }
        _logger.LogInformation("Scanning complete. Found {PluginCount} plugins.", _pluginPaths.Count);
    }
    
    private (IPluginInterface, PluginAssemblyLoadContext) LoadPlugin(string pluginPath)
    {
        var loadContext = new PluginAssemblyLoadContext(Guid.NewGuid().ToString());
        try
        {
            var assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(pluginPath));
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeof(IPluginInterface).IsAssignableFrom(type))
                {
                    return ((IPluginInterface)Activator.CreateInstance(type), loadContext);
                }
            }
        }
        catch (BadImageFormatException e)
        {
            if (Path.GetExtension(pluginPath) != ".dll" || RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var library = loadContext.LoadUnmanagedLibraryFromPath(Path.GetFullPath(pluginPath));
                if (library != IntPtr.Zero)
                {
                    return (new PluginNative(library), loadContext);
                }
            }
            
            _logger.LogError(e, "Failed to load plugin: {PluginPath}", pluginPath);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load plugin: {PluginPath}", pluginPath);
        }
        
        // no plugin found. Unload the context
        loadContext.Unload();
        
        return (null, null);
    }
    
    private struct PluginInfo : IDisposable
    {
        public IPluginInterface Interface { get; }
        public PluginAssemblyLoadContext LoadContext { get; }
        
        public PluginInfo(IPluginInterface @interface, PluginAssemblyLoadContext loadContext)
        {
            Interface = @interface;
            LoadContext = loadContext;
        }
        
        public void Dispose()
        {
            Interface.Dispose();
            LoadContext.Dispose();
        }
    }
    
    private static string PluginExtension => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? ".dll" 
        : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) 
            ? ".dylib" 
            : ".so";

    public void Dispose()
    {
        foreach (var plugin in _plugins)
            plugin.Dispose();
        
        _plugins.Clear();
    }
}

public static class PluginServiceExtensions
{
    public static void AddPlugins(this IServiceCollection services)
    {
        services.AddSingleton<PluginService>();
    }
    
    public static void UsePlugins(this IEndpointRouteBuilder app)
    {
        app.ServiceProvider.GetRequiredService<PluginService>().MapPlugins(app);
    }
}