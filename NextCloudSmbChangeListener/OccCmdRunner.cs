using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener;

public interface IOccCmdRunner
{
    Task<BufferedCommandResult?> RunOccCommandAsync(string cmd,
        CancellationToken cancellationToken = default, params string[] args);

    Task<string?> RunAndGetJsonAsync(string cmd, CancellationToken cancellationToken = default, params string[] args);
}

public class OccCmdRunner(string dockerContainer, ILogger<OccCmdRunner> logger) : IOccCmdRunner
{
    private const string
          Wrap = "docker"
        , User = "0"
        ;

    public async Task<BufferedCommandResult?> RunOccCommandAsync(string cmd,
        CancellationToken cancellationToken = default, params string[] args)
    {
        string occArgs = BuildArgs(args);
        string fullCmd = $"exec -u {User} {dockerContainer} php occ {cmd} {occArgs}";

        try
        {
            logger.LogInformation("Выполняется команда: {cmd}", fullCmd);

            var result = await Cli.Wrap(Wrap)
                .WithArguments($"exec -u {User} {dockerContainer} php occ {cmd} {occArgs}")
                .WithValidation(CommandResultValidation.None) // не кидай исключения при ненулевом exit code
                .ExecuteBufferedAsync(cancellationToken);

            logger.LogInformation("Команда {Cmd}.\r\nВыходной код: {ExitCode}.\r\nSTDOUT: {Stdout}.\r\nSTDERR: {Stderr}", fullCmd, result.ExitCode, result.StandardOutput.Trim(), result.StandardError.Trim());

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при выполнении команды OCC: {Cmd}", fullCmd);
            return null;
        }
    }

    public async Task<string?> RunAndGetJsonAsync(string cmd, CancellationToken cancellationToken = default, params string[] args)
    {
        const string 
              requiredArg = "output=json"
            ;

        var result = await RunOccCommandAsync(cmd, cancellationToken, args.Union([requiredArg]).ToArray());
        return result is not { ExitCode: 0 } ? null : result.StandardOutput;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private string BuildArgs(params string[] args)
    {
        const string
            argPrefix = "--"
            ;

        var processedArgs = args.Select(x => x.Trim('-').Trim()).Distinct().ToArray();
        var result = argPrefix + string.Join($" {argPrefix}", processedArgs);

        return result;
    }
}