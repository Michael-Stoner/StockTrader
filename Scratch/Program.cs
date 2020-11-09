using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Scratch
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //var builder = new ConfigurationBuilder()
            //                    .SetBasePath(Directory.GetCurrentDirectory())
            //                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //var baseUrl = builder.Build().GetSection("BaseUrl").Value;

            var client = new HttpClient();
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var a = await client.GetStringAsync("https://localhost:5051/api/Approaches");

        }
    }
}