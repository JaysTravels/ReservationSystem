using ReservationSystem.Domain.Models.Deeplink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Repositories
{
    public interface IDeeplinkRepository
    {
        public Task<DeeplinkResponse?> GetDeepLink();
    }
}
