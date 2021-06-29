using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rema.Infrastructure.Email.Templates;
using Serilog;

namespace Rema.Infrastructure.Email
{
  public interface IEmailTrigger
  {
    public bool SendEmail(EmailTemplate template, string recipient, IList<string> groups, bool informAcknowledgers = false);
  }

  public class EmailTrigger: IEmailTrigger
  {
    private readonly EmailSettings _emailSettings;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public EmailTrigger(EmailSettings emailSettings, IConfiguration configuration)
    {
      this._emailSettings = emailSettings;
      this._configuration = configuration;
    }

    public bool SendEmail(EmailTemplate template, string recipient, IList<string> groups, bool informAcknowledgers = false)
    {
      if (!this._emailSettings.SendEmails)
      {
        Log.Information("email sending is deactivated");
        return true;
      }
      
      if (groups == null)
      {
        groups = new List<string>();
      }
      if (recipient != null)
      {
        groups.Add(recipient);
      }
      if(informAcknowledgers)
      {
        string acknowledgeMail = this._configuration["RequestAcknowledgeEmail"];
        if(!string.IsNullOrEmpty(acknowledgeMail))
        {
          groups.Add(acknowledgeMail);
        }
      }
      if (groups.Count == 0 )
      {
        return true;
      }


      var senderEmail = "support@ovg.jm.rlp.de";
      var uniqueList = groups.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();


      MailMessage mail = new MailMessage();
      uniqueList.ForEach(e => mail.To.Add(e));
      mail.From = new MailAddress(senderEmail);
      mail.Subject = $"[{this._configuration["SiteTitle"]}] {template.Subject}";
      mail.Body = template.ToString();

      mail.IsBodyHtml = false;

        try
        {
          using (SmtpClient smtp = new SmtpClient(this._emailSettings.Domain))
          {
           // smtp.ServicePoint.MaxIdleTime = 1;
           // smtp.ServicePoint.ConnectionLimit = 1;
            smtp.EnableSsl = this._emailSettings.EnableSSL;
            smtp.Port = this._emailSettings.Port;

            if (this._emailSettings.useIISAccount)
            {
              senderEmail = this._emailSettings.EmailSenderAddress;
              smtp.UseDefaultCredentials = false;
              smtp.Credentials = new NetworkCredential(this._emailSettings.EmailUsername, this._emailSettings.EmailPassword);
              mail.From = new MailAddress(this._emailSettings.EmailSenderAddress);
          }

          /*Some SMTP servers require that the client be authenticated before the server sends email on its behalf. Set this property to true when 
           * this SmtpClient object should, if requested by the server, authenticate using the default credentials of the currently logged on user. 
           * For client applications, this is the desired behavior in most scenarios.

          Credentials information can also be specified using the application and machine configuration files. For more information, 
          see <mailSettings> Element (Network Settings).

          If the UseDefaultCredentials property is set to false, then the value set in the Credentials property will be used for the 
          credentials when connecting to the server. If the UseDefaultCredentials property is set to false and the Credentials property has not 
          been set, then mail is sent to the server anonymously.*/
          // client.UseDefaultCredentials = true;

          try
            {
              Log.Information("Sending email to " + string.Join(", ", uniqueList.ToArray()) + " @#" + template.ID);
              smtp.Send(mail);
              //await smtp.SendMailAsync(mail);
              //await smtp.SendMailAsync(senderEmail, emailAdress, $"[Raumplanung] {template.Subject}", template.ToString());
            }
            catch (Exception ex)
            {
              Log.Error(ex, $"error while sending email to {string.Join(", ", uniqueList.ToArray())} @#{template.ID}");
              return false;
            }
          }
        }catch(Exception ex)
        {
          Log.Error(ex, $"error with smtp email client connection, to {string.Join(", ", uniqueList.ToArray())} @#{template.ID}");
          return false;
        }
      Log.Information("Email was send");
      return true;
    }
  }
}
