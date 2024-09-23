using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace ChatServer;

public class Server
{
    private TcpListener _tcpListener = new TcpListener(IPAddress.Any, 8089);
    private Dictionary<string, Client> _clients = new Dictionary<string, Client>();
    
    
    public void AddClient(Client client)
    {
        _clients.Add(client.Id, client);
    }
    
    public async Task BroadCastMessage(string message, string id)
    {
        foreach (var (_, client) in _clients)
        {
            if (client.Id != id)
            {
                await client.Writer.WriteLineAsync(message);
                await client.Writer.FlushAsync();
            }
        }
    }

    public void RemoveClient(string id)
    {
        _clients.GetValueOrDefault(id).Close();
        _clients.Remove(id);
    }

    public async Task ProcessAsync() 
    {
        _tcpListener.Start();
        Console.WriteLine("Server online >>>");
        while (true)
        {
            TcpClient cl = await _tcpListener.AcceptTcpClientAsync();
            Client client = new Client(cl, this);
            Task.Run(client.ProcessAsync);
        }
    }
}