namespace Lockally.AspNetCore;

/// <summary>A transactional message to send through <see cref="LockallyMailSender"/>.</summary>
public class LockallyMessage
{
    public string From { get; set; } = string.Empty;
    public List<string> To { get; set; } = new();
    public List<string> Cc { get; set; } = new();
    public List<string> Bcc { get; set; } = new();

    /// <summary>Sent as a <c>Reply-To</c> header — Lockally has no reply_to field.</summary>
    public List<string> ReplyTo { get; set; } = new();

    public string? Subject { get; set; }
    public string? Text { get; set; }
    public string? Html { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public List<LockallyAttachment> Attachments { get; set; } = new();
}

/// <summary>A file attachment. Raw bytes are base64-encoded when sent.</summary>
public class LockallyAttachment
{
    public string Filename { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
