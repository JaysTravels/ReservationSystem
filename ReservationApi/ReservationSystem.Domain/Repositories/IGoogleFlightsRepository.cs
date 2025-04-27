using ReservationSystem.Domain.Models.AddPnrMulti;
using ReservationSystem.Domain.Models.Availability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Repositories
{
    public interface IGoogleFlightsRepository
    {
        public Task<AvailabilityRequest> GetFlightRequeust(string requestModel);
    }
}
