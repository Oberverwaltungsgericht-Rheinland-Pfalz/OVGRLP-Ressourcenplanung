using System;
using System.Collections.Generic;
using Rema.Infrastructure.Contracts.Stores;
using Rema.Infrastructure.Models;
using Rema.Infrastructure.Requests;

namespace Rema.ServiceLayer.Services
{
  public class AppointmentService
  {
    private readonly IAllocationStore _allocationStore;
    private readonly IUserStore _userStore;
    private readonly IRessourceStore _ressourceStore;
    private readonly IAllocationPurposeStore _allocationPurposeStore;
    private readonly IGadgetStore _gadgetStore;

    public AppointmentService(IAllocationStore allocationStore, IUserStore userStore, IRessourceStore ressourceStore, IAllocationPurposeStore allocationPurposeStore, IGadgetStore gadgetStore)
    {
      _allocationStore = allocationStore;
      _userStore = userStore;
      _ressourceStore = ressourceStore;
      _allocationPurposeStore = allocationPurposeStore;
      _gadgetStore = gadgetStore;
    }

    /// <summary>
    /// Request appointment for a given allocation purpose
    /// </summary>
    /// <param name="creatorId"></param>
    /// <param name="request"></param>
    /// <param name="ressourceId"></param>
    /// <param name="allocationPurposeId"></param>
    /// <returns></returns>
    public Allocation RequestAppointment(long creatorId, AllocationRequest request, long ressourceId, long allocationPurposeId)
    {
      var allocation = new Allocation()
      {
        CreatedAt = DateTime.Now,
        LastModified = DateTime.Now,
        From = request.From,
        To = request.To,
        IsAllDay = request.IsAllDay,
        Status = MeetingStatus.Pending
      };

      var purpuse = _allocationPurposeStore.GetAllocationPurposeById(allocationPurposeId);
      allocation.Purpose = purpuse;

      var creator = _userStore.GetUserById(creatorId);
      allocation.CreatedBy = creator;
      allocation.ApprovedBy = creator;
      var referencePerson = _userStore.GetUserById(request.ReferencePersonUserId);
      allocation.ReferencePerson = referencePerson;

      var ressource = _ressourceStore.GetRessourceById(ressourceId);
      allocation.Ressource = ressource;

      var savedAllocation = _allocationStore.CreateAllocation(allocation);
      return savedAllocation;
    }

    public Allocation RequestAppointment(long creatorId, AllocationRequest request, long ressourceId, AllocationPurposeRequest allocationPurpose)
    {
      var purpose = new AllocationPurpose();
      purpose.Title = allocationPurpose.Title;
      purpose.Description = allocationPurpose.Description;
      purpose.Notes = allocationPurpose.Notes;
      purpose.ContactPhone = allocationPurpose.ContactPhone;

      foreach (var gadgetId in allocationPurpose.Gadgets)
      {
        var gadget = _gadgetStore.GetGadgetById(gadgetId);
        //if (purpose.Gadgets == null || purpose.Gadgets.Count == 0)
        //    purpose.Gadgets = new List<Gadget>() { gadget };
        //else
        //    purpose.Gadgets.Add(gadget);
      }

      _allocationPurposeStore.CreateAllocationPurpose(purpose);

      return RequestAppointment(creatorId, request, ressourceId, purpose.Id);
    }

    public Allocation SetAppointment(long userId, AllocationRequest request, long ressourceId, IEnumerable<long> gadgetIds, long allocationPurposeId)
    {
      var purpose = _allocationPurposeStore.GetAllocationPurposeById(allocationPurposeId);
      var requestedAllocation = RequestAppointment(userId, request, ressourceId, purpose.Id);
      var approvedAppointment = ApproveAppointment(requestedAllocation.Id, userId);
      return approvedAppointment;
    }

    public Allocation SetAppointment(long userId, AllocationRequest request, long ressourceId, IEnumerable<long> gadgetIds, AllocationPurposeRequest allocationPurpose)
    {
      var requestedAllocation = RequestAppointment(userId, request, ressourceId, allocationPurpose);
      var approvedAppointment = ApproveAppointment(requestedAllocation.Id, userId);
      return SetAppointment(userId, request, ressourceId, gadgetIds, allocationPurpose);
    }

    public Allocation ApproveAppointment(long allocationId, long approverId)
    {
      var appointment = _allocationStore.GetAllocationById(allocationId);
      appointment.ApprovedAt = DateTime.Now;
      var approver = _userStore.GetUserById(approverId);
      appointment.ApprovedBy = approver;
      appointment.Status = MeetingStatus.Approved;
      _allocationStore.UpdateAllocation(appointment);
      return appointment;
    }

    public IEnumerable<Allocation> GetFixedAppointments()
    {
      return _allocationStore.GetAllocationsByStatus(MeetingStatus.Approved, null, null);
    }

    public IEnumerable<Allocation> GetFixedAppointmentsThisYear()
    {
      int year = DateTime.Now.Year;
      DateTime firstDay = new DateTime(year, 1, 1);
      DateTime lastDay = new DateTime(year, 12, 31);
      return _allocationStore.GetAllocationsByStatus(MeetingStatus.Approved, firstDay, lastDay);
    }

    public IEnumerable<Allocation> GetFixedAppointmentsThisMonth()
    {
      int year = DateTime.Now.Year;
      int month = DateTime.Now.Month;
      DateTime firstDay = new DateTime(year, month, 1);
      DateTime lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
      return _allocationStore.GetAllocationsByStatus(MeetingStatus.Approved, firstDay, lastDay);
    }

    public IEnumerable<Allocation> GetRequestedAppointments()
    {
      return _allocationStore.GetAllocationsByStatus(MeetingStatus.Pending, null, null);
    }

    public IEnumerable<Allocation> GetRequestedAppointmentsThisYear()
    {
      int year = DateTime.Now.Year;
      DateTime firstDay = new DateTime(year, 1, 1);
      DateTime lastDay = new DateTime(year, 12, 31);
      return _allocationStore.GetAllocationsByStatus(MeetingStatus.Pending, firstDay, lastDay);
    }

    public IEnumerable<Allocation> GetRequestedAppointmentsThisMonth()
    {
      int year = DateTime.Now.Year;
      int month = DateTime.Now.Month;
      DateTime firstDay = new DateTime(year, month, 1);
      DateTime lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
      return _allocationStore.GetAllocationsByStatus(MeetingStatus.Pending, firstDay, lastDay);
    }

    public Allocation GetDetails(long id)
    {
      return _allocationStore.GetAllocationById(id);
    }

    public IEnumerable<AllocationPurpose> GetAvailablePurposes()
    {
      return _allocationPurposeStore.GetPurposes();
    }

    public IEnumerable<Gadget> GetAllGadgets()
    {
      return _gadgetStore.GetGadgets();
    }
  }
}
