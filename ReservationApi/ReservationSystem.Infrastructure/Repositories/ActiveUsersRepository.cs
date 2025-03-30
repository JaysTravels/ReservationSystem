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
using ReservationSystem.Domain.Models.ActiveUsers;
using Org.BouncyCastle.Utilities.Net;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Domain.DBContext;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class ActiveUsersRepository : IActiveUsersRepository
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private readonly IHelperRepository _helperRepository;
        private readonly DB_Context _dbRepository;

        public ActiveUsersRepository(IConfiguration _configuration, IMemoryCache cache, IHelperRepository helperRepository, DB_Context dBRepository)
        {
            configuration = _configuration;
            _cache = cache;
            _helperRepository = helperRepository;
            _dbRepository = dBRepository;

        }

        public async Task<string> TrackUsers(ActiveUserRequest request)
        {
           
            try
            {

                string sql = @"
            INSERT INTO active_users (session_id, user_id, ip_address, last_active)
            VALUES ({0}, {1}, {2}, NOW())
            ON CONFLICT (session_id) 
            DO UPDATE SET last_active = NOW()"
                ;

                await _dbRepository.Database.ExecuteSqlRawAsync(sql, request.sessionId, request.userId, request.ip);
            }
            catch (Exception ex)
            {
                return ($"Error while saving active users {ex.Message.ToString()}");
            }
            return "OK";
        }

        public async Task<string> ClearUsers()
        {

            try
            {

                string sql = @"DELETE FROM active_users WHERE last_active < NOW() - INTERVAL '5 minutes'";

                await _dbRepository.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                return ($"Error while clean users {ex.Message.ToString()}");
            }
            return "OK";
        }

    }
}
