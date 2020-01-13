using System.Net.Mail;

namespace Rema.Infrastructure.Email
{
  public static class EmailTrigger
  {
    public static void SendEmail(string subject, string body, string recipient)
    {
      SmtpClient smtp = new SmtpClient("outlook.jmrlp.de");
      smtp.EnableSsl = false;
      smtp.Port = 25;
      smtp.Send("support@ovg.jm.rlp.de", recipient, $"[Raumplanung] {subject}", body);
    }
  }
}
