using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NextCloudSmbChangeListener.Factories;
using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NextCloudSmbChangeListener
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .MinimumLevel.Debug() // уровень логирования
                        .WriteTo.Console()    // вывод в консоль
                        .WriteTo.File(
                            path: "Logs/app-.log",    // файлы будут храниться в папке Logs
                            rollingInterval: RollingInterval.Day, // ежедневная ротация файлов
                            retainedFileCountLimit: 7,            // хранить последние 7 файлов
                            fileSizeLimitBytes: 10_000_000,       // максимум ~10MB на файл
                            rollOnFileSizeLimit: true,            // новый файл при превышении размера
                            shared: true                          // позволяет нескольким процессам писать в файл
                        );
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<NotifyWorker>();
                    services.AddSingleton<IOccCmdRunnerFactory, OccCmdRunnerFactory>();
                    services.AddSingleton<IMountListenerFactory, MountListenerFactory>();
                });

            if (isService)
            {
                builder.UseWindowsService();
            }
            else
            {
                builder.UseConsoleLifetime();
            }

            using var host = builder.Build();

            RegisterConsoleShutdown(host.Services.GetRequiredService<IHostApplicationLifetime>());

            await host.RunAsync();
        }
        private static void RegisterConsoleShutdown(IHostApplicationLifetime lifetime)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                lifetime.StopApplication(); 
            };
        }
    }
}
