using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QRCoder;

namespace Gumo.Infrastructure.Services;

public class EmailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "GuMo";
}

public class EmailService(EmailSettings settings)
{
    public async Task SendOrderConfirmationAsync(
        int orderId, string customerName, string customerEmail,
        string locationName, IEnumerable<(string Name, int Quantity, decimal UnitPrice)> items,
        string trackingUrl)
    {
        var itemList = items.ToList();
        var total = itemList.Sum(i => i.UnitPrice * i.Quantity);
        var itemsHtml = string.Join("", itemList.Select(i =>
            $"<tr><td style='padding:6px 0;color:#555'>{i.Name} × {i.Quantity}</td><td style='padding:6px 0;text-align:right;color:#555'>€{(i.UnitPrice * i.Quantity):0.00}</td></tr>"));

        var html = $"""
            <div style="font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;max-width:560px;margin:0 auto;color:#2a2a2a">
                <div style="text-align:center;padding:2rem 0">
                    <img src="https://sd-media.simplydelivery.io/65bce3103e8b3/webshop-dev/images/brand.png" width="64" height="64" style="border-radius:50%" alt="GuMo"/>
                </div>
                <h1 style="font-size:1.6rem;margin-bottom:.5rem">Order confirmed ☕</h1>
                <p style="color:#666">Hi {customerName}, your order at <strong>{locationName}</strong> is confirmed.</p>
                <table style="width:100%;border-top:1px solid #eee;border-bottom:1px solid #eee;margin:1.5rem 0;padding:.5rem 0">
                    {itemsHtml}
                    <tr><td style='padding-top:1rem;font-weight:700'>Total</td><td style='padding-top:1rem;text-align:right;font-weight:700'>€{total:0.00}</td></tr>
                </table>
                <p style="color:#666;font-size:.9rem">💳 Pay at the counter when you pick up.</p>
                <div style="text-align:center;margin:2rem 0">
                    <a href="{trackingUrl}" style="background:#2a2a2a;color:#fff;padding:.85rem 2rem;border-radius:8px;text-decoration:none;font-weight:500">
                        Track your order →
                    </a>
                </div>
                <p style="color:#999;font-size:.8rem;text-align:center">GuMo Cologne · <a href="https://www.instagram.com/gumo_club/" style="color:#999">@gumo_club</a></p>
            </div>
            """;

        var message = BuildMessage(customerEmail, $"Order #{orderId} confirmed — GuMo", html);
        await SendAsync(message);
    }

    public async Task SendEventBookingConfirmationAsync(
        string customerName, string customerEmail, int numberOfSeats,
        string confirmationCode, string eventTitle, DateTime eventDate, string locationName)
    {
        var qrPng = GenerateQrCode(confirmationCode);
        const string qrCid = "qrcode@gumo";

        var html = $"""
            <div style="font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;max-width:560px;margin:0 auto;color:#2a2a2a">
                <div style="text-align:center;padding:2rem 0">
                    <img src="https://sd-media.simplydelivery.io/65bce3103e8b3/webshop-dev/images/brand.png" width="64" height="64" style="border-radius:50%" alt="GuMo"/>
                </div>
                <h1 style="font-size:1.6rem;margin-bottom:.5rem">You're in! 🎉</h1>
                <p style="color:#666">Hi {customerName}, your booking for <strong>{eventTitle}</strong> is confirmed.</p>
                <table style="width:100%;margin:1.5rem 0">
                    <tr><td style="color:#888;font-size:.85rem;padding:4px 0">Event</td><td style="font-weight:500">{eventTitle}</td></tr>
                    <tr><td style="color:#888;font-size:.85rem;padding:4px 0">Date</td><td>{eventDate:dddd, MMMM d, yyyy · HH:mm}</td></tr>
                    <tr><td style="color:#888;font-size:.85rem;padding:4px 0">Location</td><td>{locationName}</td></tr>
                    <tr><td style="color:#888;font-size:.85rem;padding:4px 0">Seats</td><td>{numberOfSeats}</td></tr>
                </table>
                <div style="text-align:center;background:#f7f7f6;border-radius:12px;padding:2rem;margin:1.5rem 0">
                    <p style="font-size:.85rem;color:#888;margin-bottom:1rem">Your entry QR code — show this at the door</p>
                    <img src="cid:{qrCid}" width="200" height="200" alt="QR Code" style="border-radius:8px;display:block;margin:0 auto"/>
                    <p style="font-family:monospace;font-size:1.1rem;letter-spacing:.15em;margin-top:1rem;font-weight:700">{confirmationCode}</p>
                    <p style="font-size:.78rem;color:#aaa;margin-top:.5rem">Single use only — do not share</p>
                </div>
                <p style="color:#999;font-size:.8rem;text-align:center">GuMo Cologne · <a href="https://www.instagram.com/gumo_club/" style="color:#999">@gumo_club</a></p>
            </div>
            """;

        var message = BuildMessage(customerEmail, $"You're in — {eventTitle} · GuMo", html);

        // Attach QR code as inline image so email clients render it reliably
        var qrPart = new MimePart("image", "png")
        {
            Content = new MimeContent(new MemoryStream(qrPng)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
            ContentTransferEncoding = ContentEncoding.Base64,
            ContentId = qrCid,
            FileName = "qrcode.png"
        };

        var related = new MultipartRelated { message.Body!, qrPart };
        message.Body = related;

        await SendAsync(message);
    }

    private static byte[] GenerateQrCode(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(10);
    }

    private MimeMessage BuildMessage(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };
        return message;
    }

    private async Task SendAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(settings.Username, settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}