using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ChatServer;

public class Serializer : IEnumerable<Client>
{
    public static List<Client> _Clients = new List<Client>();
    private static string path = "../../../client.json";

    public static JsonSerializerOptions op = new JsonSerializerOptions()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    public static void SaveClient() => File.WriteAllText(path, JsonSerializer.Serialize(_Clients, op));

    public static List<Client> GetClients()
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "[]");
            Console.WriteLine("------------В файле ничего нету-----------");
            SaveClient();
        }
        else
        {
            _Clients = JsonSerializer.Deserialize<List<Client>>(File.ReadAllText(path));
            if (_Clients.Count == 0)
            {
                Console.WriteLine("------------В файле ничего нету-----------");
                SaveClient();
            }
        }

        return _Clients;
    }
    
    public IEnumerator<Client> GetEnumerator()
    {
        for (int i = 0; i < _Clients.Count; i++)
        {
            yield return _Clients[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}