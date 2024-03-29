﻿// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rema.DbAccess;
using Rema.WebApi.Filter;
using Rema.ServiceLayer.Services;
using Rema.Infrastructure.LDAP;
using Rema.ServiceLayer.Jobs;
using System.Threading.Tasks;

namespace Rema.WebApi.Controllers
{
  [Produces("application/json")]
  [Route("[controller]")]
  [ApiController]
  public class DebugController : BaseController
  {
    private IAdService _adService;
    private readonly ISupporterRemindJob _supporterRemind;
    private readonly IRemindJob _remindJob;
    public DebugController(RpDbContext context, IMapper mapper, IAdService adService, ISupporterRemindJob supporterRemindJob, IRemindJob remindJob) : base(context, mapper)
    {
      this._adService = adService;
      this._supporterRemind = supporterRemindJob;
      this._remindJob = remindJob;
    }

    [Route("rememberSupporters")]
    [HttpGet]
    public async Task<ActionResult> TriggerSupporterReminder()
    {
      try
      {
        await this._supporterRemind.Execute(null);
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [Route("rememberRequesters")]
    [HttpGet]
    public async Task<ActionResult> TriggerAllocationReminder()
    {
      try
      {
        await this._remindJob.Execute(null);
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [Route("CurrentUserMembers")]
    [HttpGet]
    public IEnumerable<string> GetCurrentUserMembers()
    {
      var securityMembers = new List<string>();
      var user = this.HttpContext.User;
      var identity = (WindowsIdentity)user.Identity;

      securityMembers.Add(identity.Name);
      foreach (var group in identity.Groups)
      {
        string groupTitle = group.Translate(typeof(NTAccount)).ToString();
        securityMembers.Add(groupTitle);
      }

      return securityMembers;
    }

    [HttpGet("UserMembers/{AdName}")]
    [AuthorizeAd("Admin")]
    public IEnumerable<string> GetUserMembers(string AdName)
    {
      var adService = new AdService();
      var securityMembers = new List<string>();

      if (!AdName.Contains("@"))
      {
        securityMembers.Add("Bitte geben Sie den Active-Directory Name im Format 'name@domäne' an!");
        return securityMembers;
      }
      string username = AdName.Split('@')[0].ToString();
      string domainName = AdName.Split('@')[1].ToString();

      Domain domain = adService.FindDomainByName(domainName);
      if (null == domain)
      {
        securityMembers.Add(string.Format("Es konnte keine Domäne zu '{0}' ermittelt werden!", domainName));
        return securityMembers;
      }

      securityMembers = adService.GetMemberListByUserName(domain.Name, username);

      return securityMembers;
    }

    [HttpGet("adUser")]
    [AuthorizeAd("Admin")]
    public IEnumerable<string> GetAllAdUsers()
    {
      List<string> adUsers = _adService.SearchAdUsers<string>("");

      return adUsers;
    }

    [HttpGet("adUser/{namePart}")]
    [AuthorizeAd("Admin")]
    public IEnumerable<AdUserViewModel> GetAdUsers(string namePart)
    {
      //List<string> adUsers = _adService.SearchAdUsers<string>(namePart);
      List<AdUserViewModel> adUsers = _adService.SearchAdUsers<AdUserViewModel>(namePart);

      return adUsers;
    }
  }
}
