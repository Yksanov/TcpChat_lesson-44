namespace ChatServer;

class Program
{
    static async Task Main(string[] args)
    {
        var server = new Server();
        await server.ProcessAsync();
    }
}