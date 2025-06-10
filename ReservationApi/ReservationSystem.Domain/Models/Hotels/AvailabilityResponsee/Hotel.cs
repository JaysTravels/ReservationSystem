using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
    public class Hotel
    {
        public int code { get; set; }
        public string name { get; set; }
        public int exclusiveDeal { get; set; }
        public string categoryCode { get; set; }
        public string categoryName { get; set; }
        public string destinationCode { get; set; }
        public string destinationName { get; set; }
        public int zoneCode { get; set; }
        public string zoneName { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public List<Room> rooms { get; set; }
        public string minRate { get; set; }
        public string maxRate { get; set; }
        public string currency { get; set; }
    }
}
