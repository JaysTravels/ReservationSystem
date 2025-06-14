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
      
    }
   
    public class Error
    {
        public string? errorText { get; set; }
        public string? errorCode { get; set; }
    }
}
