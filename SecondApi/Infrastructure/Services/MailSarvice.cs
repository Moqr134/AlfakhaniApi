using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using Infrastructure.Logger;

namespace Infrastructure.Services;

public class MailService
{
    private readonly string From = "profintec22@gmail.com";
    private readonly string Pass = "ttfr pobi btal zasj";

    public string SendMail(string To, string Subject, string Body)
    {
        MailMessage Message = new MailMessage
        {
            Subject = Subject,
            Body = Body,
            From = new MailAddress(From),
            IsBodyHtml = true,
            Priority = MailPriority.High
        };

        SmtpClient smtp = new SmtpClient
        {
            Port = 587,
            EnableSsl = true,
            Host = "smtp.gmail.com",
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(From, Pass)
        };

        Message.To.Add(new MailAddress(To));

        try
        {
            smtp.Send(Message);

            return "تم إرسال البريد بِنجاح";
        }
        catch (Exception ex)
        {
            new Loger().Write(ex, "Send Mail");
            //return "لم تكتمل عملية إرسال البريد";
            throw;
        }
    }
}
