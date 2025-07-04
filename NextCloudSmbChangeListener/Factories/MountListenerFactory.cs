using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener.Factories;

/// <summary>
/// Создаёт экземпляры <see cref="IMountListener"/>.
/// </summary>
public interface IMountListenerFactory
{
    /// <summary>
    /// Создаёт слушателя для указанного подключения.
    /// </summary>
    IMountListener Create(int mountId, string nextcloudContainerName);
}

/// <summary>
/// Стандартная реализация <see cref="IMountListenerFactory"/>.
/// </summary>
public class MountListenerFactory(IOccCmdRunnerFactory occCmdRunnerFactory, ILogger<MountListener> logger) : IMountListenerFactory
{
    public IMountListener Create(int mountId, string nextcloudContainerName) =>
            new MountListener(mountId, nextcloudContainerName, occCmdRunnerFactory, logger);
}
