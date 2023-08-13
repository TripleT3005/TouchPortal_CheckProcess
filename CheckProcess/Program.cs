using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TouchPortalSDK.Configuration;

namespace TouchPortalSDK
{
    class Program
    {
        static void Main(string[] args)
        {
            //Build configuration:
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)!.FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            //Standard method for build a ServiceProvider in .Net:
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure =>
            {
                configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] ");
                configure.AddConfiguration(configurationRoot.GetSection("Logging"));
            });

            //Registering the Plugin to the IoC container:
            serviceCollection.AddTouchPortalSdk(configurationRoot);
            serviceCollection.AddSingleton<CheckProcess>();

            var serviceProvider = serviceCollection.BuildServiceProvider(true);

            //Use your IoC framework to resolve the plugin with it's dependencies,
            // or you can use 'TouchPortalFactory.CreateClient' to get started manually:
            var plugin = serviceProvider.GetRequiredService<CheckProcess>();
            plugin.Run();
        }
    }
}