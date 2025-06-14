using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
   public class Room
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<Rate> rates { get; set; }
    }
}
