namespace BookLibrary.ServerPluginKit;

public interface IPluginInterface
{
    public delegate void HttpMap(string method, string path, Func<PluginHttpRequest, PluginHttpResponse> handler);
    
    void Setup(HttpMap map);
    void Dispose();
}

public struct PluginHttpRequest
{
    public string Method { get; init; }
    public string Path { get; init; }
    public byte[] Body { get; init; }
}

public struct PluginHttpResponse
{
    public int StatusCode { get; init; }
    public byte[] Body { get; init;  }
    public Dictionary<string, string> Headers { get; init; }
}