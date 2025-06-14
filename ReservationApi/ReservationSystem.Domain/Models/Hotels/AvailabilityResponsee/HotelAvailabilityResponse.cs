using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
    public class HotelAvailabilityResponse
    {
        public HotelResponse? HotelResponse { get; set; }
        public Error? error { get; set; }

    }
}
