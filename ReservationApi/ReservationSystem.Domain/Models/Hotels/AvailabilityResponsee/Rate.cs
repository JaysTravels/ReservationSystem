using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
    
    public class Rate
    {
        public string rateKey { get; set; }
        public string rateClass { get; set; }
        public string rateType { get; set; }
        public string net { get; set; }
        public string sellingRate { get; set; } // Nullable in original JSON
        public bool? hotelMandatory { get; set; }
        public int allotment { get; set; }
        public string paymentType { get; set; }
        public bool packaging { get; set; }
        public string boardCode { get; set; }
        public string boardName { get; set; }
        public List<CancellationPolicy> cancellationPolicies { get; set; }
        public Taxes taxes { get; set; }
        public int rooms { get; set; }
        public int adults { get; set; }
        public int children { get; set; }
        public List<Offer> offers { get; set; }
    }
}
