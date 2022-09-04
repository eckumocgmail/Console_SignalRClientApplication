using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class SignalRClient
{
    private string url;
    private string username;
    private string password;
    private HubConnection Connection;

    public SignalRClient(string url, string username, string password)
    {
        this.url = url;
        this.username = username;
        this.password = password;
    }


    public void Connect<TService>( IServiceProvider provider ) where TService : class
    {
        var actions = new ConcurrentDictionary<string, Action<string>>();
        var names = typeof(TService).GetMethods().Select(method => method.Name).Distinct();
        foreach (var name in names)
        {

            actions[name] = (message) => 
            {
                var availableMethods = typeof(TService).GetMethods().Where(m => m.Name == name);
            };
            
        }
    }

 
    public async Task Connect( 
        IDictionary<string, Action<string>> PublicApi, 
        Action OnStart )
    {
        Console.WriteLine($"Connect( ... )");
       
        this.Connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

        this.Connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await Connection.StartAsync().ContinueWith(async (result) =>
            {
                Console.WriteLine($"Connection to {url} started {result.IsFaulted} ");
                OnStart();
            });
        };
         
         
        foreach ( var kv in PublicApi )
        {
            var OnMessage = kv.Value;
            this.Connection.On<string>( kv.Key, ( message ) => {
                try
                {
                    Console.WriteLine($"ON {kv.Key} {message}");

                    OnMessage(message);
                    Console.WriteLine($"SUCCESS {kv.Key}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FAILED {kv.Key} {ex.Message}");
                }
            });
        }

        await Connection.StartAsync().ContinueWith(async (result) =>
        {
            this.Info(result.AsyncState);
        });

    }

    internal async Task<string> Signin(string username, string password)
    {
        this.Info(this.Connection.State);
        Thread.Sleep(1000);
        this.Info(this.Connection.State);

        return "123";
    }
}