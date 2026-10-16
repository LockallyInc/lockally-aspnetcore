namespace Lockally.AspNetCore;

/// <summary>Configuration for the Lockally ASP.NET Core integration.</summary>
public class LockallyOptions
{
    /// <summary>Lockally API key. <c>lk_test_</c> runs in sandbox, <c>lk_live_</c> sends real mail.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Optional base URL override (e.g. a private endpoint).</summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Default <c>from</c> address used when sending ASP.NET Core Identity emails
    /// (confirmation / password reset) through the registered <c>IEmailSender</c>.
    /// </summary>
    public string? DefaultFrom { get; set; }
}
