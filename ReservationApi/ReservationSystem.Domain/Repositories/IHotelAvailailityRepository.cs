using ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee;
using ReservationSystem.Domain.Models.Hotels.AvailabiltiyRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Repositories
{
    public interface IHotelAvailailityRepository
    {
        public Task<HotelAvailabilityResponse> GetAvailability(HotelAvailabilityRequest requestModel);
    }
}
