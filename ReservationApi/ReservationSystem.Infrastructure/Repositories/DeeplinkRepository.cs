using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReservationSystem.Domain.Models.AddPnrMulti;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Models.DBLogs;
using ReservationSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using ReservationSystem.Domain.Models.Deeplink;
using ReservationSystem.Domain.DBContext;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class DeeplinkRepository : IDeeplinkRepository
    {

        private readonly IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private readonly IHelperRepository _helperRepository;
        private readonly IDBRepository _dbRepository;
        public DeeplinkRepository(IConfiguration _configuration, IMemoryCache cache, IHelperRepository helperRepository, IDBRepository dBRepository)
        {
            configuration = _configuration;
            _cache = cache;
            _helperRepository = helperRepository;
            _dbRepository = dBRepository;

        }

        public async Task<DeeplinkResponse?> GetDeepLink()
        {
            DeeplinkResponse response = new DeeplinkResponse();
            try
            {
                var DeepLinkList = await _dbRepository.GetDeeplink();
                response.deepLinkModels = DeepLinkList;
                response.Error = "";
              
            }
            catch (Exception ex)
            {
                response.Error = "Error " + ex.Message.ToString();
                return response;
            }
            return response;
        }
    }
}
