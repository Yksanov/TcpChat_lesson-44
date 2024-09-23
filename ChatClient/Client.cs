using System.Net.Sockets;

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
            Console.WriteLine("Welcome to the chat");
            var receiveTask = ReceiveMessagesAsync(reader);
            var sendTask = SendMessagesAsync(writer, userName);
            await Task.WhenAny(receiveTask, sendTask);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            client.Close();
        }
    }

    private async Task SendMessagesAsync(StreamWriter writer, string? userName)
    {
        await writer.WriteLineAsync($"{userName}");
        await writer.FlushAsync();
        Console.WriteLine("Enter your message:");
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