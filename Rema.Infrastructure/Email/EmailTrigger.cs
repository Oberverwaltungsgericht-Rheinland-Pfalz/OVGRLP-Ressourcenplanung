using System.Collections.Generic;
using System.Linq;
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
      if (recipient != null) groups.Add(recipient);
      var uniqueList = groups.Distinct().ToList();
      foreach (var emailAdress in uniqueList)
      {
        SmtpClient smtp = new SmtpClient("outlook.jmrlp.de");
        smtp.EnableSsl = false;
        smtp.Port = 25;
        smtp.Send("support@ovg.jm.rlp.de", emailAdress, $"[Ressourcenplanungssystem] {template.Subject}", template.ToString());
      }
    }
  }
}
