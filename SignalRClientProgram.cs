using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using static InputApplicationProgram;

internal class SignalRClientProgram
{
    public static string Url { get; private set; }
    public static string Username { get; private set; }
    public static string Password { get; private set; }

    static void Main(params string[] args)
    {
        if(Environment.UserInteractive)
        {
            
            Url = "https://localhost:5001/hubs/clients";//"InputString(ref args, "Url");
            Username = InputString(ref args, "Username");
            Password = InputString(ref args, "Password");
        }
        else
        {
            if(args.Length != 3)
            {
                throw new ArgumentException("Необходимо передать 3 аргумента: URL,login,password");
            }
            else
            {
                Url = args[0];
                Username = args[1];
                Password = args[2];
            }
        }
        Start(Url, Username, Password).Wait();
        Thread.Sleep(Timeout.Infinite);

    }

    private static async Task Start(string url, string username, string password)
    {
        Console.WriteLine($"Start( {url},  {username},  {password} )");
        var actions = new ConcurrentDictionary<string, Action<string>>();
        actions["WriteLine"] = (message) => 
        {
            Console.WriteLine(message);
        };
        var client = new SignalRClient(url, username, password);
        await client.Connect(actions, async () => {

            while(String.IsNullOrWhiteSpace(await client.Signin(username, password)))
            {

            }
        });
    }

     
}
 
