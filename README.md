# Lockally.AspNetCore

Official [Lockally](https://lockally.com) integration for ASP.NET Core. Register Lockally
in DI, inject `LockallyMailSender` to send transactional email, and let ASP.NET Core
Identity send its confirmation and password-reset emails through Lockally.

## Install

```bash
dotnet add package Lockally.AspNetCore
```

## Register

```csharp
// Program.cs
builder.Services.AddLockally(o =>
{
    o.ApiKey = builder.Configuration["Lockally:ApiKey"]!;   // lk_test_… runs in sandbox
    o.DefaultFrom = "alerts@yourdomain.com";                 // used for Identity emails
    // o.BaseUrl = "https://api.lockally.com";                // optional override
});
```

This registers `LockallyMailSender` and an `IEmailSender<TUser>` implementation, so
ASP.NET Core Identity’s account-confirmation and password-reset emails are delivered by
Lockally automatically.

## Send

```csharp
public class SignupController : ControllerBase
{
    private readonly LockallyMailSender _mailer;

    public SignupController(LockallyMailSender mailer) => _mailer = mailer;

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(string email)
    {
        var message = new LockallyMessage
        {
            From = "alerts@yourdomain.com",
            Subject = "Welcome",
            Html = "<b>Thanks for signing up.</b>",
        };
        message.To.Add(email);
        message.ReplyTo.Add("support@yourdomain.com");
        await _mailer.SendAsync(message);
        return Accepted();
    }
}
```

### Mapping notes

- **Reply-To** is sent as a header (Lockally has no `reply_to` field).
- **Attachments** are base64-encoded as `{ filename, contentType, contentBase64 }`.
- One `Idempotency-Key` is generated per send (24 h dedupe window).

## License

MIT © Lockally
