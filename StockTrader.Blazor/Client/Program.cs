using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Blazored.LocalStorage;

namespace StockTrader.Blazor.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzIxMjI1QDMxMzgyZTMyMmUzMGRqdy9PRU5UN0JRZUZ1Zi9BVVVaNVRYR25ZZHh4TTlsbVhrcHhRUzVGeFk9");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();

            //UI components
            builder.Services.AddSyncfusionBlazor();

            await builder.Build().RunAsync();

        }
    }
}