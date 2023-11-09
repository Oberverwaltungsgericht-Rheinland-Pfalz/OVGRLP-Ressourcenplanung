// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
  public interface IRemindJob : IJob { }

  public class RemindJob : IRemindJob
  {
    protected readonly RpDbContext _context;
    protected readonly IEmailTrigger _emailTrigger;
    private readonly IConfiguration _configuration;

    public RemindJob(RpDbContext context, IEmailTrigger emailTrigger, IConfiguration configuration)
    {
      this._context = context;
      this._emailTrigger = emailTrigger;
      this._configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      Log.Information("Execution of RemindJob");

      int remindRange = this._configuration.GetValue<int>("Reminder");
      var timeRange = DateTime.Now.AddDays(remindRange);
      var timeRangeDistance = DateTime.Now.AddDays(3);
      var timeRangeCreation = DateTime.Now.AddDays(-20);

      IList<Allocation> unreminded = new List<Allocation>();
      try
      {
       unreminded = await _context.Allocations
      .Include(o => o.Ressources)
      .Include(r => r.ReferencePerson)
      .Include(r => r.LastModifiedBy)
      .Include(r => r.CreatedBy)
      .Include(g => g.Gadgets).ThenInclude(g => g.SuppliedBy)
      .Where(a => a
        .ReferencePerson != null && 

        // Termin muss weniger als 10 Tage entfernt sein
        a.From < timeRange &&

        // Termin muss noch mindestens 3 Tage entfernt sein
        a.From > timeRangeDistance &&
        
        // Terminerstellung muss mindestens 20 Tage her sein 
        a.CreatedAt < timeRangeCreation
        
        // wurde noch nicht erinnert
        && !a.Reminded 
        )
      .ToListAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocations");
        return;
      }
      Log.Information("Execution of RemindJob: Found {countRemindAllocations} allocations to remind", unreminded.Count);
      int counter = 0;

      foreach (var remindAllocation in unreminded)
      {
        var template = new RemindTemplate(remindAllocation);
        bool reminded = _emailTrigger.SendEmail(template, null, new List<string>(){ remindAllocation.ReferencePerson.Email });

        remindAllocation.Reminded = reminded;
        counter += reminded ? 1 : 0;
      }
      Log.Information("Execution of RemindJob: Send {countRemindEmails} emails for {countRemindAllocations} allocations to remind", 
        counter, unreminded.Count);

      try
      {
        await _context.SaveChangesAsync();
      } catch (Exception ex)
      {
        Log.Error(ex, "error while saving reminded allocations");
      }
      Log.Information("Execution of RemindJob: saved remined allocations");
    }
  }
}
