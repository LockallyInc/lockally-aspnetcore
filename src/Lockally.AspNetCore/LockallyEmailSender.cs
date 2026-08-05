using Microsoft.AspNetCore.Identity;

namespace Lockally.AspNetCore;

/// <summary>
/// ASP.NET Core Identity email sender backed by Lockally. Registered by <c>AddLockally</c>,
/// it delivers the confirmation and password-reset emails Identity sends. Requires
/// <see cref="LockallyOptions.DefaultFrom"/> to be set.
/// </summary>
public class LockallyEmailSender<TUser> : IEmailSender<TUser> where TUser : class
{
    private readonly LockallyMailSender _mailer;
    private readonly LockallyOptions _options;

    public LockallyEmailSender(LockallyMailSender mailer, LockallyOptions options)
    {
        _mailer = mailer;
        _options = options;
    }

    public Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink) =>
        Send(email, "Confirm your email",
            $"<p>Please confirm your account by <a href=\"{confirmationLink}\">clicking here</a>.</p>");

    public Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink) =>
        Send(email, "Reset your password",
            $"<p>Reset your password by <a href=\"{resetLink}\">clicking here</a>.</p>");

    public Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode) =>
        Send(email, "Reset your password",
            $"<p>Your password reset code is: <strong>{resetCode}</strong></p>");

    private Task Send(string email, string subject, string html)
    {
        if (string.IsNullOrEmpty(_options.DefaultFrom))
        {
            throw new InvalidOperationException(
                "Set LockallyOptions.DefaultFrom to send ASP.NET Core Identity emails via Lockally.");
        }

        var message = new LockallyMessage { From = _options.DefaultFrom!, Subject = subject, Html = html };
        message.To.Add(email);
        return _mailer.SendAsync(message);
    }
}
