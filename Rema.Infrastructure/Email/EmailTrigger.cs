using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Rema.Infrastructure.Email.Templates;
using Serilog;

namespace Rema.Infrastructure.Email
{
  public interface IEmailTrigger
  {
    public void SendEmail(EmailTemplate template, string recipient, IList<string> groups);
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

    public void SendEmail(EmailTemplate template, string recipient, IList<string> groups)
    {
      if (!this._emailSettings.SendEmails)
      {
        Log.Information("email sending to deaktivated");
        return;
      }
      if (recipient != null)
      {
        groups.Add(recipient);
      }

      var senderEmail = "support@ovg.jm.rlp.de";
      var uniqueList = groups.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
      foreach (var emailAdress in uniqueList)
      {
        SmtpClient smtp = new SmtpClient(this._emailSettings.Domain);
        smtp.EnableSsl = this._emailSettings.EnableSSL;
        smtp.Port = this._emailSettings.Port;

        if (this._emailSettings.useIISAccount)
        {
          senderEmail = this._emailSettings.EmailUsername;
          smtp.UseDefaultCredentials = false;
          smtp.Credentials = new NetworkCredential(this._emailSettings.EmailUsername, this._emailSettings.EmailPassword);
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
          Log.Information("Sending email to "+ emailAdress);
          smtp.Send(senderEmail, emailAdress, $"[Raumplanung] {template.Subject}", template.ToString());
        }
        catch (Exception ex)
        {
          Log.Error(ex, $"error while sending email to {emailAdress}");
          return;
        }
      }
    }
  }
}
