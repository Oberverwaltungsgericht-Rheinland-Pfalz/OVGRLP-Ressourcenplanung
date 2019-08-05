using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbRaumplanung.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.ServiceContracts;

namespace AspNetCoreVueStarter.Controllers
{
    public class AppointmentController : ControllerBase
    {
        private readonly IRessourceService _ressourceService;

        public AppointmentController(IRessourceService ressourceService)
        {
            _ressourceService = ressourceService;
        }

        public IEnumerable<Allocation> GetAllocations(long ressourceId, DateTime? from, DateTime? to)
        {
            return _ressourceService.GetAllocations(ressourceId, from, to);
        }
        public IEnumerable<Gadget> GetAssignedGadgets(long ressourceId)
        {
            return _ressourceService.GetAssignedGadgets(ressourceId);
        }
        public Ressource GetDetails(long id)
        {
            return _ressourceService.GetDetails(id);
        }
    }
}