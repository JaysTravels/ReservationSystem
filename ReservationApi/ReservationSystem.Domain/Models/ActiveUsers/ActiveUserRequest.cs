using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.ActiveUsers
{
   public class ActiveUserRequest
    {
        public string? sessionId { get; set; }
        public string? userId { get; set; }
        public string? ip { get; set; }

    }
}
