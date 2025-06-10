using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
   public class HotelTax
    {
        public bool included { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string type { get; set; }
    }
}
