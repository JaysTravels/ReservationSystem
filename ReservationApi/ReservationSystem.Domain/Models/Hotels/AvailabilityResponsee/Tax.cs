using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
   public class Tax
    {
        public bool included { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public string clientAmount { get; set; }
        public string clientCurrency { get; set; }
        public string percent { get; set; }
    }
}
