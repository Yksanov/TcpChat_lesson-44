using System.Net.Security;
using System.Net.Sockets;

namespace ChatServer;

public class Client
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamReader Reader { get; }
    protected internal StreamWriter Writer { get; }

    private TcpClient _client;
    private Server _server;

    public Client(TcpClient client, Server server)
    {
        _client = client;
        _server = server;

        var stream = _client.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);

        server.AddClient(this);
    }

    public async Task ProcessAsync()
    {
        try
        {
            string? userName = await Reader.ReadLineAsync();
            string? message = $"{userName} join the chat";
            await _server.BroadCastMessage(message, Id);
            Console.WriteLine(message);
            try
            {
                while (true)
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    message = $" > {userName} : {message}";
                    Console.WriteLine(message);
                    await _server.BroadCastMessage(message, Id);
                }
            }
            catch (Exception ex)
            {
                message = $"{userName} left the chat";
                Console.WriteLine(message);
                await _server.BroadCastMessage(message, Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _server.RemoveClient(Id);
        }
    }

    public void Close()
    {
        Writer.Close();
        Reader.Close();
        _client.Close();
    }
}