using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quartz;
using Rema.DbAccess;
using Rema.Infrastructure.Email;
using Rema.Infrastructure.Email.Templates;
using Rema.Infrastructure.Models;
using Serilog;

namespace Rema.ServiceLayer.Jobs
{
  public interface ISupporterRemindJob : IJob { }

  public class SupporterRemindJob : ISupporterRemindJob
  {
    protected readonly RpDbContext _context;
//    protected readonly IMapper _mapper;
    protected readonly IEmailTrigger _emailTrigger;
    private readonly IConfiguration _configuration;

    public SupporterRemindJob(RpDbContext context, IEmailTrigger emailTrigger, IConfiguration configuration)
    {
      this._context = context;
      this._emailTrigger = emailTrigger;
      this._configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      Log.Information("Execution of SupporterRemindJob");

      var timeRange = DateTime.Now.AddHours(24);
      var timeRangeCreation = DateTime.Now.AddDays(-3);

      var relevantGroups = await _context.SupplierGroups.Where(s => s.Remind).ToListAsync();
      var relevantGroupIds = relevantGroups.Select(s => s.Id).ToList();

      var groupMails = new Dictionary<long, string>();
      foreach(var group in relevantGroups)
      {
        groupMails.Add(group.Id, group.GroupEmail);
      }
      
      IList<Allocation> unreminded = new List<Allocation>();
      try
      {
       unreminded = await _context.Allocations
      .Include(r => r.CreatedBy)
      .Include(g => g.Gadgets).ThenInclude(g => g.SuppliedBy)
      .Where(a => 
        // Termin muss weniger als 24 Stunden entfernt sein
        a.From < timeRange &&
      
        // Terminerstellung muss mindestens 3 Tage her sein 
        a.CreatedAt < timeRangeCreation
        
        // wurde noch nicht erinnert
        && !a.SupportersReminded 

        // enthält Hilfsmittel, welche von Unterstützergruppen bereitgestellt werden, welche erinnert werden müssen
        && (a.Gadgets != null || a.HintsForSuppliers.Any())
        && ((a.Gadgets == null ? 0 : a.Gadgets.Count) + a.HintsForSuppliers.Count > 0)

        // da auch Termine mit Nachrichten an Unterstützergruppen erinnert werden müssen wäre dies zu genau
        // die Überprüfung erfolgt in der Schleife
        // && (a.Gadgets.Where(g => g.SuppliedBy.Remind).Count() > 0)
        )
      .ToListAsync();

      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocations");
        return;
      }

      var mails4Allocation = new Dictionary<Allocation, List<string>>();
      for (var i = 0; i < unreminded.Count; i++) { 
        var remindAllocation = unreminded[i];
        var mailList = new List<string>();
      
        var remindBecauseOfGadgets = remindAllocation.Gadgets.Where(g => { 
          var checkResult = relevantGroupIds.Contains(g.SuppliedBy.Id);
          if (checkResult) mailList.Add(g.SuppliedBy.GroupEmail);
          return checkResult;
        });
        var remindBecauseOfMessage = remindAllocation.HintsForSuppliers.Where(g => {
          var checkResult = relevantGroupIds.Contains(g.Group.Id);
          if (checkResult) mailList.Add(g.Group.GroupEmail);
          return checkResult;
        });
        
        if(mailList.Count() > 0)
        {
          mails4Allocation.Add(remindAllocation, mailList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList());
        }
      }
      Log.Information("Execution of SupporterRemindJob: Found {countSupporterReminders} allocations to remind", mails4Allocation.Count);

      int counter = 0;
      foreach(var remindAllocation in mails4Allocation)
      {
        var template = new SupportRemindTemplate(remindAllocation.Key);
        bool reminded = _emailTrigger.SendEmail(template, null, remindAllocation.Value);

        remindAllocation.Key.SupportersReminded = reminded;
        if (reminded) counter++;
      }
      Log.Information("Execution of SupporterRemindJob: Send {countSupportRemindEmails} emails for {countSupportRemindAllocations} allocations to remind",
  counter, mails4Allocation.Count);

      try
      {
        await _context.SaveChangesAsync();
      } catch (Exception ex)
      {
        Log.Error(ex, "error while saving reminded allocations");
      }
      Log.Information("Execution of SupporterRemindJob: saved changes");
    }
  }
}
