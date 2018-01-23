using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
//using ScheduleApp.Web.EndpointConfiguration;
//using ScheduleApp.Web.KestrelServerOptionsExtensions;

namespace ScheduleApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if (isLinux)
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseKestrel(options => options.ConfigureEndpoints())
                    .Build();
            }
            else
            {
                return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .Build();
            }
        }

    }
}
