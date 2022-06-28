using ScreenCaptureService;
using ScreenCaptureService.Db;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;

namespace Company.WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {

            ImageServer server = new ImageServer();
            server.Start(8888);

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ScreenCapture>();
                })
                .Build();
            
            host.Run();

            server.Stop();
        }
    }
}