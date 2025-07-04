using Microsoft.Extensions.Logging;
using NextCloudSmbChangeListener.Factories;
using Serilog.Core;

namespace NextCloudSmbChangeListener;

/// <summary>
/// Интерфейс слушателя для отслеживания изменений конкретного подключения.
/// </summary>
public interface IMountListener
{
    int Id { get; }
    Task? Task { get; }
    /// <summary>
    /// Запускает прослушивание изменений на подключении.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Событие завершения работы слушателя.
    /// </summary>
    event EventHandler<EventArgs> Completed;
}

/// <summary>
/// Реализация <see cref="IMountListener"/>, запускающая OCC-команды уведомлений.
/// </summary>
public class MountListener(int mountId, string nextcloudContainerName, IOccCmdRunnerFactory occCmdRunnerFactory, ILogger<MountListener> logger) : IMountListener
{
    private readonly IOccCmdRunner _cmdRunner = occCmdRunnerFactory.Create(nextcloudContainerName);

    /// <summary>
    /// Идентификатор подключения.
    /// </summary>
    public int Id => mountId;

    /// <summary>
    /// Задача слушателя, если он запущен.
    /// </summary>
    public Task? Task { get; private set; }

    /// <summary>
    /// Признак работы слушателя.
    /// </summary>
    public bool IsRunning => Task is not null && !Task.IsCompleted;

    /// <summary>
    /// Событие, возникающее по завершении работы слушателя.
    /// </summary>
    public event EventHandler<EventArgs>? Completed;

    /// <inheritdoc />
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
