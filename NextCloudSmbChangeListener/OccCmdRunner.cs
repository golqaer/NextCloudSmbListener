using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener;

public interface IOccCmdRunner
{
    Task<BufferedCommandResult?> RunOccCommandAsync(string cmd, string occArgs,
        CancellationToken cancellationToken = default);

    Task<string?> RunAndGetJsonAsync(string cmd, CancellationToken cancellationToken = default, params string[] args);
    string BuildArgs(params string[] args);
}

public class OccCmdRunner(string dockerContainer, ILogger<OccCmdRunner> logger) : IOccCmdRunner
{
    private const string
          Wrap = "docker"
        , User = "0"
        ;

    public async Task<BufferedCommandResult?> RunOccCommandAsync(string cmd, string occArgs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Выполняется команда: occ {cmd} {OccArgs}", cmd, occArgs);

            var result = await Cli.Wrap(Wrap)
                .WithArguments($"exec -u {User} {dockerContainer} php occ {cmd} {occArgs}")
                .WithValidation(CommandResultValidation.None) // не кидай исключения при ненулевом exit code
                .ExecuteBufferedAsync(cancellationToken);

            logger.LogInformation("Выходной код: {ExitCode}", result.ExitCode);
            logger.LogDebug("STDOUT: {Stdout}", result.StandardOutput.Trim());
            logger.LogDebug("STDERR: {Stderr}", result.StandardError.Trim());

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при выполнении команды OCC: {Cmd} {OccArgs}", cmd, occArgs);
            return null;
        }
    }

    public async Task<string?> RunAndGetJsonAsync(string cmd, CancellationToken cancellationToken = default, params string[] args)
    {
        const string 
              requiredArg = "output=json"
            ;
        var occArgs = BuildArgs(args.Union([requiredArg]).ToArray());

        var result = await RunOccCommandAsync(cmd, occArgs, cancellationToken);
        return result is not { ExitCode: 0 } ? null : result.StandardOutput;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    public string BuildArgs(params string[] args)
    {
        const string
            argPrefix = "--"
            ;

        var processedArgs = args.Select(x => x.Trim('-').Trim()).Distinct().ToArray();
        var result = argPrefix + string.Join($" {argPrefix}", processedArgs);

        return result;
    }
}