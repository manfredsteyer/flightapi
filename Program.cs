using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace flight_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Uncomment to Create DB Schema
            // using(var ctx = new FlightContext()) {
            //      ctx.Database.EnsureCreated();
            // }

            var seeder = new DatabaseSeeder();
            seeder.SeedDatabase();

            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
