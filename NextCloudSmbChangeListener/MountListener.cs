using Microsoft.Extensions.Logging;
using NextCloudSmbChangeListener.Factories;
using Serilog.Core;

namespace NextCloudSmbChangeListener;

public interface IMountListener
{
    int Id { get; }
    Task? Task { get; }
    Task StartAsync(CancellationToken ct = default);

    event EventHandler<EventArgs> Completed;
}

public class MountListener(int mountId, string nextcloudContainerName, IOccCmdRunnerFactory occCmdRunnerFactory, ILogger<MountListener> logger) : IMountListener
{
    private readonly IOccCmdRunner _cmdRunner = occCmdRunnerFactory.Create(nextcloudContainerName);

    public int Id => mountId;
    public Task? Task { get; private set; }
    public bool IsRunning => Task is not null && !Task.IsCompleted;

    public event EventHandler<EventArgs>? Completed;

    public async Task StartAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Запускаю listener для mount {Id}", Id);

        try
        {
            Task = _cmdRunner.RunAndGetJsonAsync($"files_external:notify {Id}", ct, "no-self-check");
            await Task;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка в listener для mount {Id}", Id);
        }
        finally
        {
            logger.LogInformation("Listener для mount {Id} завершен", Id);
            Task = null;
            Completed?.Invoke(this, EventArgs.Empty);
        }

    }
}