using System;

namespace BookLibrary.Server.Options;

public class PluginOptions
{
    public string Directory { get; set; } = "Plugins";
    public string[] OnlyPlugins { get; set; } = Array.Empty<string>();
}