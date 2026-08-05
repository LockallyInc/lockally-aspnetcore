using System.Text;
using Lockally.AspNetCore;
using Xunit;

public class MappingTests
{
    [Fact]
    public void MapsMessageToSendRequest()
    {
        var msg = new LockallyMessage
        {
            From = "alerts@acme.com",
            Subject = "Your code",
            Text = "code 123",
            Html = "<b>code 123</b>",
        };
        msg.To.Add("user@example.com");
        msg.Cc.Add("cc@example.com");
        msg.ReplyTo.Add("reply@acme.com");
        msg.Headers["X-Campaign"] = "welcome";
        msg.Attachments.Add(new LockallyAttachment
        {
            Filename = "invoice.pdf",
            ContentType = "application/pdf",
            Content = Encoding.UTF8.GetBytes("BYTES"),
        });

        var req = LockallyMailSender.ToRequest(msg);

        Assert.Equal("alerts@acme.com", req.From);
        Assert.Equal(new List<string> { "user@example.com" }, req.To);
        Assert.Equal(new List<string> { "cc@example.com" }, req.Cc);
        Assert.Equal("Your code", req.Subject);
        Assert.Equal("code 123", req.Text);
        Assert.Equal("<b>code 123</b>", req.Html);
        Assert.Equal("reply@acme.com", req.Headers!["Reply-To"]);
        Assert.Equal("welcome", req.Headers!["X-Campaign"]);
        Assert.Single(req.Attachments);
        Assert.Equal("invoice.pdf", req.Attachments![0].Filename);
        Assert.Equal("application/pdf", req.Attachments![0].ContentType);
        Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes("BYTES")),
            req.Attachments![0].ContentBase64);
    }

    [Fact]
    public void OmitsOptionalFieldsWhenAbsent()
    {
        var msg = new LockallyMessage { From = "a@acme.com", Text = "yo" };
        msg.To.Add("b@example.com");

        var req = LockallyMailSender.ToRequest(msg);

        Assert.Equal(new List<string> { "b@example.com" }, req.To);
        Assert.Equal("yo", req.Text);
        Assert.Null(req.Cc);
        Assert.Null(req.Html);
        Assert.Null(req.Headers);
        Assert.Null(req.Attachments);
    }
}
