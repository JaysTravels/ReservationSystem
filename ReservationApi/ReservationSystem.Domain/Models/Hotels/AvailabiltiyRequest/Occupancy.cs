using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabiltiyRequest
{
    public class Occupancy
    {
        public int Rooms { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}
