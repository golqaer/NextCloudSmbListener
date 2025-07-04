using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Serilog;
using NextCloudSmbChangeListener.Factories;

namespace NextCloudSmbChangeListener
{
    internal class Program
    {
        static void Main(string[] args)
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

            builder.Build().Run();
        }
    }
}
