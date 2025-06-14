using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabiltiyRequest
{
    public class HotelAvailabilityRequest
    {
        public Stay stay { get; set; }
        public List<Occupancy> occupancies { get; set; }
        public Destination destination { get; set; }
    }
}
