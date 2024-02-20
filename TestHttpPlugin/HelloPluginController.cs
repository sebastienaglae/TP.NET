using System.Text;
using BookLibrary.ServerPluginKit;

namespace TestHttpPlugin;

public class HelloPluginController : IPluginInterface
{
    public void Setup(IPluginInterface.HttpMap map)
    {
        map("GET", "/hello", OnHello);
        map("POST", "/hello", OnHelloPost);
        map("GET", "/goodbye", OnGoodbye);
    }

    public void Dispose()
    {
    }
    
    private PluginHttpResponse OnHello(PluginHttpRequest request)
    {
        return new PluginHttpResponse
        {
            StatusCode = 200,
            Body = "Hello, World!"u8.ToArray(),
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "text/plain"
            }
        };
    }
    
    private PluginHttpResponse OnHelloPost(PluginHttpRequest request)
    {
        return new PluginHttpResponse
        {
            StatusCode = 200,
            Body = Encoding.UTF8.GetBytes("Hello, World!\nYou posted: " + Encoding.UTF8.GetString(request.Body).Trim()),
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "text/plain"
            }
        };
    }
    
    private PluginHttpResponse OnGoodbye(PluginHttpRequest request)
    {
        return new PluginHttpResponse
        {
            StatusCode = 200,
            Body = "Goodbye, World!"u8.ToArray(),
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "text/plain"
            }
        };
    }
}