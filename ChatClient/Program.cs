using System.Runtime.CompilerServices;

namespace ChatClient;

class Program
{
    static async Task Main(string[] args)
    {
        Client client = new Client();
        string host = "";
        string post = "";

        while (true)
        {
            try
            {
                Console.WriteLine("Enter host:");
                host = Console.ReadLine();
                Console.WriteLine("Enter port:");
                post = Console.ReadLine();
                await client.RunAsync(host, Convert.ToInt32(post));
                Serializer.SaveClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error, try again!");
            }
        }
    }
}