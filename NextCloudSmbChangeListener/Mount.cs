using Newtonsoft.Json;

namespace NextCloudSmbChangeListener;

public class Mount
{
    [JsonProperty("mount_id")]
    public int MountId { get; set; }

    [JsonProperty("mount_point")]
    public string MountPoint { get; set; } = default!;

    [JsonProperty("storage")]
    public string Storage { get; set; } = default!;

    [JsonProperty("authentication_type")]
    public string AuthenticationType { get; set; } = default!;

    [JsonProperty("configuration")]
    public MountConfiguration Configuration { get; set; } = default!;

    [JsonProperty("options")]
    public MountOptions Options { get; set; } = default!;
}

public class MountConfiguration
{
    [JsonProperty("share")]
    public string Share { get; set; } = default!;

    [JsonProperty("root")]
    public string Root { get; set; } = default!;

    [JsonProperty("domain")]
    public string Domain { get; set; } = default!;

    [JsonProperty("show_hidden")]
    public bool ShowHidden { get; set; }

    [JsonProperty("case_sensitive")]
    public bool CaseSensitive { get; set; }

    [JsonProperty("check_acl")]
    public bool CheckAcl { get; set; }

    [JsonProperty("timeout")]
    public string Timeout { get; set; } = default!;

    [JsonProperty("user")]
    public string User { get; set; } = default!;

    [JsonProperty("host")]
    public string Host { get; set; } = default!;

    [JsonProperty("password")]
    public string Password { get; set; } = default!;
}

public class MountOptions
{
    [JsonProperty("encrypt")]
    public bool Encrypt { get; set; }

    [JsonProperty("previews")]
    public bool Previews { get; set; }

    [JsonProperty("enable_sharing")]
    public bool EnableSharing { get; set; }

    [JsonProperty("filesystem_check_changes")]
    public int FilesystemCheckChanges { get; set; }

    [JsonProperty("encoding_compatibility")]
    public bool EncodingCompatibility { get; set; }

    [JsonProperty("readonly")]
    public bool Readonly { get; set; }
}