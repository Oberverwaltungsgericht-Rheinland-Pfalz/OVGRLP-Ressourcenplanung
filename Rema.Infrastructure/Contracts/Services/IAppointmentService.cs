using System.Collections.Generic;
using Rema.Infrastructure.Models;
using Rema.Infrastructure.Requests;

namespace Rema.Infrastructure.Contracts.Services
{
  public interface IAppointmentService
  {
    Allocation RequestAppointment(long creatorId, AllocationRequest request, long ressourceId, long allocationPurposeId);

    Allocation RequestAppointment(long creatorId, AllocationRequest request, long ressourceId, AllocationPurposeRequest allocationPurpose);

    Allocation SetAppointment(long userId, AllocationRequest request, long ressourceId, IEnumerable<long> gadgetIds, long allocationPurposeId);

    Allocation SetAppointment(long userId, AllocationRequest request, long ressourceId, IEnumerable<long> gadgetIds, AllocationPurposeRequest allocationPurpose);

    Allocation ApproveAppointment(long allocationId, long approverId);

    IEnumerable<Allocation> GetFixedAppointments();

    IEnumerable<Allocation> GetFixedAppointmentsThisYear();

    IEnumerable<Allocation> GetFixedAppointmentsThisMonth();

    IEnumerable<Allocation> GetRequestedAppointments();

    IEnumerable<Allocation> GetRequestedAppointmentsThisYear();

    IEnumerable<Allocation> GetRequestedAppointmentsThisMonth();

    Allocation GetDetails(long id);

    IEnumerable<Gadget> GetAllGadgets();
  }
}
