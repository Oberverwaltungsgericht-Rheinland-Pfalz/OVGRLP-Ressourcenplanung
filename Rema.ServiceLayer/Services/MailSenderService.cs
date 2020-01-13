namespace Rema.ServiceLayer.Services
{
  public class MailSenderService
  {
    /*        private readonly ConfigurationService configurationService;

            public MailSenderService(ConfigurationService configurationService)
            {
                if (configurationService == null)
                    throw new ArgumentNullException("configurationService");

                this.configurationService = configurationService;
            }

            public void SendInfoMail(OVGRLPMessage message)
            {
                GmmLaufzettelAccessService gmmLaufzettelAccessService = new GmmLaufzettelAccessService(message);

                String smtpHost = configurationService.GetValue("mailsettings/smtphost");
                String from = configurationService.GetValue("mailsettings/from");
                String to = configurationService.GetValue(gmmLaufzettelAccessService.RecipientGmmId, "infomail/to");
                String subject = configurationService.GetValue(gmmLaufzettelAccessService.RecipientGmmId, "infomail/subject");
                String body = configurationService.GetValue(gmmLaufzettelAccessService.RecipientGmmId, "infomail/body");

                subject = subject.Replace("#subject#", gmmLaufzettelAccessService.Subject);
                body = body.Replace("#sender#",
                  String.IsNullOrEmpty(gmmLaufzettelAccessService.SenderName) ?
                  gmmLaufzettelAccessService.SenderAddress :
                  gmmLaufzettelAccessService.SenderName + " (" + gmmLaufzettelAccessService.SenderGmmId + ")");
                body = body.Replace("#recipient#", gmmLaufzettelAccessService.RecipientName + " (" + gmmLaufzettelAccessService.RecipientGmmId + ")");
                body = body.Replace("#subject#", gmmLaufzettelAccessService.Subject);

                MailMessage msg = new MailMessage(from, to, subject, body);

                SmtpClient smtpClient = new SmtpClient(smtpHost);
                smtpClient.Send(msg);
            }

            public void SendErrorMail(string errorMessage)
            {
                String smtpHost = configurationService.GetValue("mailsettings/smtphost");
                String from = configurationService.GetValue("mailsettings/from");
                String to = configurationService.GetValue("errormail/to");
                String subject = configurationService.GetValue("errormail/subject");
                String body = configurationService.GetValue("errormail/body");

                body = body.Replace("#errormessage#", errorMessage);

                MailMessage message = new MailMessage(from, to, subject, body);
                message.Priority = MailPriority.High;

                SmtpClient smtpClient = new SmtpClient(smtpHost);
                smtpClient.Send(message);
            }*/
  }
}
