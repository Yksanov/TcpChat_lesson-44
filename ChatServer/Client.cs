using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;

namespace ChatServer;

public class Client
{
    public string Username { get; set; }
    public string ServerHost { get; set; } //
    public int ServerPort { get; set; } //
    public string Password { get; set; } //
    [JsonInclude]
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    [JsonIgnore]
    protected internal StreamReader Reader { get; }
    [JsonIgnore]
    protected internal StreamWriter Writer { get; }
    
    private TcpClient _client;
    private Server _server;
    

    public Client(TcpClient client, Server server,string userName, string serverHost, int serverPort)
    {
        _client = client;
        _server = server;
        
        Username = userName; //
        ServerHost = serverHost;  //
        ServerPort = serverPort; //
        
        var stream = _client.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
        Serializer.SaveClient();  //
    }

    public bool CheckName(string name)
    {
        return _server.Clients.Values.FirstOrDefault(c => c.Username == name) == null;
    }

    private string GetNames()
    {
        string names = "";
        foreach (var c in _server.Clients.Values)
        {
            names += $"{c.Username}\n";
        }

        return names;
    }
    public async Task ProcessAsync()
    {
        try
        {
            string? userName = await Reader.ReadLineAsync();
            string? password = await Reader.ReadLineAsync(); //
            while (!CheckName(userName))
            {
                _server.PrivateMessage("Name is already taken. Try another name!", Id);
                string names = GetNames();
                _server.PrivateMessage("Users in chat:\n" + names, Id);
                userName = await Reader.ReadLineAsync();
                password = await Reader.ReadLineAsync();
            }
            
            Username = userName;
            Password = password; //
            ServerHost = _server.serverHost; //
            ServerPort = _server.serverPort; //
            
            _server.AddClient(this);
            Serializer.SaveClient(); //
            
            _server.PrivateMessage("For enter message type your message and press enter\nFor private message use this format: ->[name], [name]: [your message]", Id);
            string? message = $"{userName} join the chat ({DateTime.Now.Hour}:{DateTime.Now.Minute})";
            await _server.BroadCastMessage(message, Id);
            Console.WriteLine(message);
            try
            {
                while (true)
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    Console.WriteLine(message);
                    if (message.StartsWith("->"))
                    {
                        string[] namesWithMessage = message.Substring(2).Split(":");
                        string[] namesToSend = namesWithMessage[0].Split(",");
                        foreach (var n in namesToSend)
                        {
                            Client c = _server.Clients.Values.FirstOrDefault(x => x.Username == n);
                            if (c != null)
                            {
                                message = $" <{userName}> : {namesWithMessage}" + 
                                          $" ({DateTime.Now.Hour}:{DateTime.Now.Minute})";
                                _server.PrivateMessage(message, c.Id);
                            }
                            else
                            {
                                _server.PrivateMessage($"There is no user with that name: {n}", c.Id);
                            }
                        }
                    }
                    else
                    {
                        message = $" < {userName} > : {message}" + 
                                  $" ({DateTime.Now.Hour}:{DateTime.Now.Minute})";
                        await _server.BroadCastMessage(message, Id);
                    }
                    
                    Task time = Task.Delay(100000); // Task-2
                    Task<string> timeout = Reader.ReadLineAsync();
                    Task t = await Task.WhenAny(time, timeout);
            
                    if (t == time)
                    {
                        Console.WriteLine("===== Time: Server OutSide! =====");
                        _client.Close();
                        return;
                    }
                    message = await timeout;

                    if (message == null)
                    {
                        Console.WriteLine("===== Server OutSide! ===== ");
                        _client.Close();
                        return;
                    }
                    
                    Serializer.SaveClient(); // 
                }
            }
            catch (Exception ex)
            {
                message = $"{userName} left the chat ({DateTime.Now.Hour}:{DateTime.Now.Minute})";
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
            Serializer.SaveClient();  //
        }
    }

    public void Close()
    {
        Writer.Close();
        Reader.Close();
        _client.Close();
    }

    public bool CheckPassword(string pass)
    {
        Serializer.GetClients();
        return _server.Clients.Values.FirstOrDefault(c => c.Password == pass) == null;
    }
}