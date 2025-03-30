using ReservationSystem.Domain.Models.ActiveUsers;
using ReservationSystem.Domain.Models.AddPnrMulti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Repositories
{
   public interface IActiveUsersRepository
    {
        public Task<string> TrackUsers(ActiveUserRequest request);

        public Task<string> ClearUsers();
    }
}
