using Newtonsoft.Json;

namespace NextCloudSmbChangeListener;

/// <summary>
/// Представляет внешнее подключение, настроенное в Nextcloud.
/// </summary>
public class Mount
{
    [JsonProperty("mount_id")]
    /// <summary>
    /// Идентификатор точки монтирования.
    /// </summary>
    public int MountId { get; set; }

    [JsonProperty("mount_point")]
    /// <summary>
    /// Путь, по которому подключено хранилище.
    /// </summary>
    public string MountPoint { get; set; } = default!;

    [JsonProperty("storage")]
    /// <summary>
    /// Тип используемого хранилища.
    /// </summary>
    public string Storage { get; set; } = default!;

    [JsonProperty("authentication_type")]
    /// <summary>
    /// Тип аутентификации, используемый для подключения.
    /// </summary>
    public string AuthenticationType { get; set; } = default!;

    [JsonProperty("configuration")]
    /// <summary>
    /// Настройки подключения.
    /// </summary>
    public MountConfiguration Configuration { get; set; } = default!;

    [JsonProperty("options")]
    /// <summary>
    /// Дополнительные параметры монтирования.
    /// </summary>
    public MountOptions Options { get; set; } = default!;
}

/// <summary>
/// Настройки SMB‑подключения.
/// </summary>
public class MountConfiguration
{
    [JsonProperty("share")]
    /// <summary>
    /// Путь удалённой общедоступной папки.
    /// </summary>
    public string Share { get; set; } = default!;

    [JsonProperty("root")]
    /// <summary>
    /// Корневая папка подключения.
    /// </summary>
    public string Root { get; set; } = default!;

    [JsonProperty("domain")]
    /// <summary>
    /// Домен для аутентификации.
    /// </summary>
    public string Domain { get; set; } = default!;

    [JsonProperty("show_hidden")]
    /// <summary>
    /// Показывать скрытые файлы.
    /// </summary>
    public bool ShowHidden { get; set; }

    [JsonProperty("case_sensitive")]
    /// <summary>
    /// Учитывать регистр имён файлов.
    /// </summary>
    public bool CaseSensitive { get; set; }

    [JsonProperty("check_acl")]
    /// <summary>
    /// Проверять ACL.
    /// </summary>
    public bool CheckAcl { get; set; }

    [JsonProperty("timeout")]
    /// <summary>
    /// Таймаут подключения.
    /// </summary>
    public string Timeout { get; set; } = default!;

    [JsonProperty("user")]
    /// <summary>
    /// Имя пользователя для подключения.
    /// </summary>
    public string User { get; set; } = default!;

    [JsonProperty("host")]
    /// <summary>
    /// Адрес удалённого сервера.
    /// </summary>
    public string Host { get; set; } = default!;

    [JsonProperty("password")]
    /// <summary>
    /// Пароль для подключения.
    /// </summary>
    public string Password { get; set; } = default!;
}

/// <summary>
/// Дополнительные опции подключения.
/// </summary>
public class MountOptions
{
    [JsonProperty("encrypt")]
    /// <summary>
    /// Использовать шифрование.
    /// </summary>
    public bool Encrypt { get; set; }

    [JsonProperty("previews")]
    /// <summary>
    /// Генерировать превью.
    /// </summary>
    public bool Previews { get; set; }

    [JsonProperty("enable_sharing")]
    /// <summary>
    /// Разрешить совместный доступ.
    /// </summary>
    public bool EnableSharing { get; set; }

    [JsonProperty("filesystem_check_changes")]
    /// <summary>
    /// Интервал проверки изменений.
    /// </summary>
    public int FilesystemCheckChanges { get; set; }

    [JsonProperty("encoding_compatibility")]
    /// <summary>
    /// Включить совместимость кодировок.
    /// </summary>
    public bool EncodingCompatibility { get; set; }

    [JsonProperty("readonly")]
    /// <summary>
    /// Только чтение.
    /// </summary>
    public bool Readonly { get; set; }
}
