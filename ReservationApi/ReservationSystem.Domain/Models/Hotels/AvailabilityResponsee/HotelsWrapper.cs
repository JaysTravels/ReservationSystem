using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
    public class HotelsWrapper
    {
        public List<Hotel> hotels { get; set; }
        public string checkIn { get; set; }
        public int total { get; set; }
        public string checkOut { get; set; }
    }
    public class Root
    {
        public HotelsWrapper hotels { get; set; }
    }
}
