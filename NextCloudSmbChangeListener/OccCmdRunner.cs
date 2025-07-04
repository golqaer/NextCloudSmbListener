using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace NextCloudSmbChangeListener;

/// <summary>
/// Предоставляет методы для выполнения OCC-команд в контейнере Docker.
/// </summary>
public interface IOccCmdRunner
{
    /// <summary>
    /// Выполняет OCC-команду и возвращает необработанный результат.
    /// </summary>
    /// <param name="cmd">Имя команды.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="args">Дополнительные аргументы.</param>
    Task<BufferedCommandResult?> RunOccCommandAsync(string cmd,
        CancellationToken cancellationToken = default, params string[] args);

    /// <summary>
    /// Выполняет OCC-команду и ожидает вывод в формате JSON.
    /// </summary>
    /// <param name="cmd">Имя команды.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="args">Дополнительные аргументы.</param>
    /// <returns>JSON-строка или null в случае ошибки.</returns>
    Task<string?> RunAndGetJsonAsync(string cmd, CancellationToken cancellationToken = default, params string[] args);
}

/// <summary>
/// Исполняет OCC-команды внутри Docker-контейнера с помощью CliWrap.
/// </summary>
public class OccCmdRunner(string dockerContainer, ILogger<OccCmdRunner> logger) : IOccCmdRunner
{
    private const string
          Wrap = "docker"
        , User = "0"
        ;

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<string?> RunAndGetJsonAsync(string cmd, CancellationToken cancellationToken = default, params string[] args)
    {
        const string 
              requiredArg = "output=json"
            ;

        var result = await RunOccCommandAsync(cmd, cancellationToken, args.Union([requiredArg]).ToArray());
        return result is not { ExitCode: 0 } ? null : result.StandardOutput;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    /// <summary>
    /// Формирует аргументы командной строки в формате OCC.
    /// </summary>
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
