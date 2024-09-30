using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace ChatServer;

public class Server
{
    public Dictionary<string, Client> Clients => _clients;
    private TcpListener _tcpListener = new TcpListener(IPAddress.Any, 8089);
    private static Dictionary<string, Client> _clients = new Dictionary<string, Client>();
    public string serverHost { get; set; } //
    public int serverPort { get; set; }  //
    public string userName { get; set; } //
    
    public void AddClient(Client client)
    {
        _clients.Add(client.Id, client);
        Serializer._Clients.Add(client); //
        Serializer.SaveClient(); //
    }

    public async Task PrivateMessage(string message, string id)
    {
        Client? client = _clients.GetValueOrDefault(id);
        if (client != null)
        {
            await client.Writer.WriteLineAsync(message);
            await client.Writer.FlushAsync();
        }
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

        // Serializer._Clients.Remove(_clients.GetValueOrDefault(id));
        Serializer.SaveClient();
    }

    public async Task ProcessAsync() 
    {
        _tcpListener.Start();
        Console.WriteLine("Server online >>>");
        while (true)
        {
            TcpClient cl = await _tcpListener.AcceptTcpClientAsync();
            Client client = new Client(cl, this,userName, serverHost, serverPort); // 
            Task.Run(client.ProcessAsync);
        }
    }
}