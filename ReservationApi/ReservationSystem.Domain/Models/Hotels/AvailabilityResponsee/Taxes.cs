using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
   public class Taxes
    {
        public List<Tax> taxes { get; set; }
        public bool allIncluded { get; set; }
    }
}
