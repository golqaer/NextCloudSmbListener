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
    OccCmdRunner Create(string dockerContainer);
}

/// <summary>
/// Стандартная реализация <see cref="IOccCmdRunnerFactory"/>.
/// </summary>
public class OccCmdRunnerFactory(ILogger<OccCmdRunner> logger) : IOccCmdRunnerFactory
{
    public OccCmdRunner Create(string dockerContainer)
        => new (dockerContainer, logger);
}
