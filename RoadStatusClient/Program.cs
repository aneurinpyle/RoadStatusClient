using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoadStatusClient.Model;
using RoadStatusClient.Service;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RoadStatusClient
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main()
        {
            RegisterServices();

            Console.WriteLine("TfL Major Road Status Client\r");
            Console.WriteLine("----------------------------\n");

            Console.WriteLine("Please enter a major road or roads, such as 'A2' or 'A2,A20', to get each road's current status.");
            Console.WriteLine("Or leave blank to return all major roads and their statuses");
            Console.Write("Road(s): ");
            var roadIds = Console.ReadLine();

            var clientService = _serviceProvider.GetService<IRoadStatusService>();
            await clientService.RetreiveRoadStatus(roadIds);
            
            DisposeServices();
        }


        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IRoadStatusService, RoadStatusService>();
            services.AddHttpClient();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var appSettings = new ApplicationConfiguration();
            configuration.Bind("ApplicationSettings", appSettings);
            services.AddSingleton(appSettings);

            _serviceProvider = services.BuildServiceProvider();
        }
        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
