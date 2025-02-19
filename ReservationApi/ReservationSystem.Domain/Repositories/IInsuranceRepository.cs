using ReservationSystem.Domain.Models.Enquiry;
using ReservationSystem.Domain.Models.Insurance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Repositories
{
    public interface IInsuranceRepository
    {
        public Task<bool> SendInsuranceReqest(InsuranceRequest requestModel);
    }
}
