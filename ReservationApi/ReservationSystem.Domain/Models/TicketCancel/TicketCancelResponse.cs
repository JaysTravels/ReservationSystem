﻿using ReservationSystem.Domain.Models.Availability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.TicketCancel
{
    public class TicketCancelResponse
    {
        public HeaderSession? session { get; set; }
        public object? responseDetails { get; set; }
        public AmadeusResponseError? amadeusError { get; set; }
    }
}
