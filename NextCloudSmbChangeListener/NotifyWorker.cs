using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextCloudSmbChangeListener.Factories;

namespace NextCloudSmbChangeListener;

public class NotifyWorker(ILogger<NotifyWorker> logger, IOccCmdRunnerFactory occCmdRunnerFactory, IMountListenerFactory mountListenerFactory)
    : BackgroundService
{
    private const string NextcloudContainerName = "nextcloud";
    private const int PeriodInMinutes = 1;

    private static readonly string[] mountAllowFilter =
    [
        "password::password"
    ];

    private readonly IOccCmdRunner _cmdRunner = occCmdRunnerFactory.Create(NextcloudContainerName);

    private ICollection<IMountListener> _mountListeners = new List<IMountListener>();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        const string noMountsMsg = "Не удалось получить список монтирований.";

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var json = await _cmdRunner.RunAndGetJsonAsync("files_external:list", ct, "all");
                _ = json ?? throw new InvalidOperationException(noMountsMsg);

                var mounts = JsonConvert.DeserializeObject<Mount[]>(json);
                _ = mounts ?? throw new InvalidOperationException(noMountsMsg);

                var tasks = new List<Task>();
                foreach (var mount in mounts.Where(m => mountAllowFilter.Contains(m.AuthenticationType)))
                {
                    if (_mountListeners.Any(m => m.Id == mount.MountId))
                    {
                        logger.LogInformation("Listener для mount {Id} уже существует, пропускаю", mount.MountId);
                        continue;
                    }

                    var listener = mountListenerFactory.Create(mount.MountId, NextcloudContainerName);
                    listener.Completed += OnCompleteMountListener;
                    _mountListeners.Add(listener);
                    tasks.Add(listener.StartAsync(ct));
                }

                await Task.Delay(TimeSpan.FromMinutes(PeriodInMinutes), ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        await Task.WhenAll(_mountListeners.Select(x => x.Task)!);
    }

    private void OnCompleteMountListener(object? sender, EventArgs e)
    {
        if (sender is IMountListener listener)
        {
            _mountListeners.Remove(listener);
            logger.LogInformation("Listener для mount {Id} удален из списка", listener.Id);
        }
    }
}