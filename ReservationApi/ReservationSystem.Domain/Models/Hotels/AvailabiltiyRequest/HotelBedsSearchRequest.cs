using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabiltiyRequest
{
    public class HotelBedsSearchRequestModel
    {
        public Stay Stay { get; set; }
        public List<Occupancy> Occupancies { get; set; }
        public HotelCode HotelCode { get; set; }
    }
}
