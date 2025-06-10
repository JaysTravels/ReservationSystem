using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
   public class CancellationPolicy
    {
        public string amount { get; set; }
        public DateTime from { get; set; }
    }
}
