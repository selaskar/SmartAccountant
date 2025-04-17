using System.Diagnostics.CodeAnalysis;

namespace SmartAccountant.Identity.Options;

[ExcludeFromCodeCoverage]
public record IdentityOptions
{
    public const string Section = "Identity";

    public CredentialType CredentialType { get; set; }

    /// <summary>
    /// Required for <see cref="CredentialType.ManagedIdentity"/>.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Required for <see cref="CredentialType.AzureCli"/> and <see cref="CredentialType.VsCredential"/>.
    /// </summary>
    public string? TenantId { get; set; }
}

public enum CredentialType
{
    /// <summary>
    /// Supports both system-assigned and user-assigned managed identities.
    /// </summary>
    ManagedIdentity = 0,
    VsCredential = 1,
    AzureCli = 2
}
