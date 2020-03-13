using System.Collections.Generic;
using System.Net.Mail;
using Rema.Infrastructure.Email.Templates;

namespace Rema.Infrastructure.Email
{
  public static class EmailTrigger
  {
    public static void SendEmail(string subject, string body, string recipient)
    {
      SmtpClient smtp = new SmtpClient("outlook.jmrlp.de");
      smtp.EnableSsl = false;
      smtp.Port = 25;
      smtp.Send("support@ovg.jm.rlp.de", recipient, $"[Ressourcenplanungssystem] {subject}", body);
    }
    public static void SendEmail(EmailTemplate template, string recipient, IList<string> groups)
    {
      groups.Add(recipient);
      foreach (var emailAdress in groups)
      {
        SmtpClient smtp = new SmtpClient("outlook.jmrlp.de");
        smtp.EnableSsl = false;
        smtp.Port = 25;
        smtp.Send("support@ovg.jm.rlp.de", recipient, $"[Ressourcenplanungssystem] {template.Subject}", template.ToString());
      }
    }
  }
}
