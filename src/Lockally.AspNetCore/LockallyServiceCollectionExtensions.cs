using Lockally.SDK.Client;
using Lockally.SDK.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Lockally.AspNetCore;

/// <summary>DI wiring for Lockally in ASP.NET Core.</summary>
public static class LockallyServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Lockally client, a <see cref="LockallyMailSender"/>, and an
    /// ASP.NET Core Identity <c>IEmailSender&lt;TUser&gt;</c> backed by Lockally.
    ///
    /// <code>
    /// builder.Services.AddLockally(o => o.ApiKey = builder.Configuration["Lockally:ApiKey"]!);
    /// </code>
    /// </summary>
    public static IServiceCollection AddLockally(this IServiceCollection services, Action<LockallyOptions> configure)
    {
        var options = new LockallyOptions();
        configure(options);
        if (string.IsNullOrEmpty(options.ApiKey))
        {
            throw new ArgumentException("Lockally ApiKey is required.", nameof(configure));
        }

        services.AddApi(host =>
        {
            host.AddTokens(new BearerToken(options.ApiKey));
            host.UseProvider<RateLimitProvider<BearerToken>, BearerToken>();
            host.AddApiHttpClients(client =>
                client.BaseAddress = new Uri(string.IsNullOrEmpty(options.BaseUrl)
                    ? ClientUtils.BASE_ADDRESS
                    : options.BaseUrl!.TrimEnd('/')));
        });

        services.AddSingleton(options);
        services.AddScoped<LockallyMailSender>();
        services.AddScoped(typeof(IEmailSender<>), typeof(LockallyEmailSender<>));
        return services;
    }
}
