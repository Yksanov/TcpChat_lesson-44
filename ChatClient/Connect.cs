namespace ChatClient;

public class Connect
{
    public string ServerHost { get; set; } //
    public int ServerPort { get; set; } //

    public Connect(string host, int port)
    {
        ServerHost = host;
        ServerPort = port;
    }
}