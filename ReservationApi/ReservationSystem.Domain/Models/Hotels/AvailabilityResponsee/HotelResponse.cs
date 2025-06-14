using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Hotels.AvailabilityResponsee
{
    public class HotelResponse
    {
        public AuditData auditData { get; set; }
        public Hotels hotels { get; set; }
    }
}
