using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener.Factories;

/// <summary>
/// Фабрика для создания экземпляров <see cref="OccCmdRunner"/>.
/// </summary>
public interface IOccCmdRunnerFactory
{
    /// <summary>
    /// Создаёт исполнителя OCC-команд для указанного контейнера.
    /// </summary>
    IOccCmdRunner Create(string dockerContainer);
}

/// <summary>
/// Стандартная реализация <see cref="IOccCmdRunnerFactory"/>.
/// </summary>
public class OccCmdRunnerFactory(ILogger<OccCmdRunner> logger) : IOccCmdRunnerFactory
{
    public IOccCmdRunner Create(string dockerContainer)
        => new OccCmdRunner(dockerContainer, logger);
}
