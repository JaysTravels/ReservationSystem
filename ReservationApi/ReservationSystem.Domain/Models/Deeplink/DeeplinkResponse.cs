using ReservationSystem.Domain.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSystem.Domain.DB_Models;

namespace ReservationSystem.Domain.Models.Deeplink
{
   public class DeeplinkResponse
    {
        public List<ReservationSystem.Domain.DB_Models.Deeplink> deepLinkModels { get; set; }
        public string Error { get; set; }
    }
}
