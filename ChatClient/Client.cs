using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace ChatClient;

public class Client
{
    public async Task RunAsync(string host, int port)
    {
        using var client = new TcpClient();
        try
        {
            client.Connect(host, port);
            var stream = client.GetStream();
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            Console.WriteLine("Enter your name:");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter password: "); //
            string password = Console.ReadLine(); //
            Serializer.SaveGetConnect(new Connect(host, port)); //
            var receiveTask = ReceiveMessagesAsync(reader);
            var sendTask = SendMessagesAsync(writer, userName, password); //
            await Task.WhenAny(receiveTask, sendTask);
            Serializer._Clients.Add(this); //
            Serializer.SaveClient(); //
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Error, try again!");
        }
        finally
        {
            client.Close();
        }
    }

    private async Task SendMessagesAsync(StreamWriter writer, string? userName, string? password)  //
    {
        await writer.WriteLineAsync($"{userName}");
        await writer.WriteLineAsync($"{password}");
        await writer.FlushAsync();
        while (true)
        {
            string? message = Console.ReadLine();
            if(string.IsNullOrEmpty(message)) continue;
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
        }
    }

    private async Task ReceiveMessagesAsync(StreamReader reader)
    {
        while (true)
        {
            try
            {
                string? message = await reader.ReadLineAsync();
                if(string.IsNullOrEmpty(message)) continue;
                Console.WriteLine(message);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                break;
            }
        }
    }
}