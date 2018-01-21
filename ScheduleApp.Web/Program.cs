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

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options => options.ConfigureEndpoints())
                .Build();
    }
}
