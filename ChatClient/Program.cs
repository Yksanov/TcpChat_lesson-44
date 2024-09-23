using System.Runtime.CompilerServices;

namespace ChatClient;

class Program
{
    static async Task Main(string[] args)
    {
        Client client = new Client();
        await client.RunAsync("127.0.0.1", 8089);
    }
}