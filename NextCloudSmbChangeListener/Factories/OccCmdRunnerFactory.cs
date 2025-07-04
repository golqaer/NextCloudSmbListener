using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener.Factories;

public interface IOccCmdRunnerFactory
{
    OccCmdRunner Create(string dockerContainer);
}

public class OccCmdRunnerFactory(ILogger<OccCmdRunner> logger) : IOccCmdRunnerFactory
{
    public OccCmdRunner Create(string dockerContainer)
        => new (dockerContainer, logger);
}