using Lockally.SDK.Api;
using Lockally.SDK.Model;

namespace Lockally.AspNetCore;

/// <summary>
/// Sends transactional email through the Lockally API (POST /v1/send). Registered in DI
/// by <c>AddLockally</c>; inject it and call <see cref="SendAsync"/>.
/// </summary>
public class LockallyMailSender
{
    private readonly ISendApi _api;

    public LockallyMailSender(ISendApi api) => _api = api;

    /// <summary>Send one message. Throws if the API rejects it.</summary>
    public async Task SendAsync(LockallyMessage message, CancellationToken cancellationToken = default)
    {
        // One idempotency key per logical send (required header, 24 h dedupe window).
        var key = "lk-" + Guid.NewGuid().ToString("N");
        var response = await _api.V1SendPostAsync(key, ToRequest(message), cancellationToken)
            .ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Lockally send failed with status {(int)response.StatusCode}.");
        }
    }

    /// <summary>Map a <see cref="LockallyMessage"/> onto the /v1/send request. Public for testing.</summary>
    public static V1SendPostRequest ToRequest(LockallyMessage m)
    {
        var headers = new Dictionary<string, string>(m.Headers);
        if (m.ReplyTo.Count > 0)
        {
            // Lockally has no reply_to field — carry it as a header.
            headers["Reply-To"] = string.Join(", ", m.ReplyTo);
        }

        var req = new V1SendPostRequest(m.From, m.To);
        if (m.Cc.Count > 0) req.Cc = m.Cc;
        if (m.Bcc.Count > 0) req.Bcc = m.Bcc;
        if (m.Subject is not null) req.Subject = m.Subject;
        if (m.Text is not null) req.Text = m.Text;
        if (m.Html is not null) req.Html = m.Html;
        if (headers.Count > 0) req.Headers = headers;
        if (m.Attachments.Count > 0)
        {
            req.Attachments = m.Attachments
                .Select(a => new V1SendPostRequestAttachmentsInner(
                    a.Filename,
                    a.ContentType ?? "application/octet-stream",
                    Convert.ToBase64String(a.Content)))
                .ToList();
        }

        return req;
    }
}
