using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WalkingTec.Mvvm.BlazorDemo.Shared
{
    public class ApiClient
    {
        public HttpClient Client { get; }

        public ApiClient(HttpClient client)
        {
            Client = client;
        }

        public ApiClient(HttpClient client, IHostEnvironment env)
        {
            if (client.BaseAddress == null && env != null)
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            }
            Client = client;
        }

    }
}
