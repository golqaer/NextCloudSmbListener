using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener.Factories;

public interface IMountListenerFactory
{
    IMountListener Create(int mountId, string nextcloudContainerName);
}

public class MountListenerFactory(IOccCmdRunnerFactory occCmdRunnerFactory, ILogger<MountListener> logger) : IMountListenerFactory
{
    public IMountListener Create(int mountId, string nextcloudContainerName) =>
            new MountListener(mountId, nextcloudContainerName, occCmdRunnerFactory, logger);
}