using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockTrader.Data;
using AutoMapper;
using StockTrader.Data.Downloader;
using StockTrader.Data.Downloader.Yahoo;
using StockTrader.Data.Repositories;
using Microsoft.Net.Http.Headers;
using StockTrader.API;

namespace StockTrader
{
    public class Startup
    {
        private static CorsPolicyConfiguration corsConfiguration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            corsConfiguration = new CorsPolicyConfiguration();
            Configuration.Bind("CorsPolicies", corsConfiguration);
            var corsPolicy = corsConfiguration.Policies.First(p => p.PolicyName.Equals(corsConfiguration.DefaultPolicyName));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                                  builder =>
                                  {
                                      builder.WithOrigins(corsPolicy.Origins);
                                      builder.AllowAnyHeader();
                                  });
            });

            services.AddDbContext<StockTraderContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("StockTrader")));

            services.AddControllers();

            services.AddScoped<IStockDataRepo, SqlStockDataRepo>();
            services.AddScoped<IStockDownloader, YahooFinanceDownloader>();
            services.AddScoped<IApproachRepo, SqlApproachRepo>();
            services.AddScoped<IRunRepo, SqlRunRepo>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //UseCors must be between UseRouting() and UseAuthorization() and before UseResponseCaching() if it is used
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}