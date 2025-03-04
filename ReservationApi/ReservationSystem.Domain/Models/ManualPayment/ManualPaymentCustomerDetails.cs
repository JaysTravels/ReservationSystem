﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.ManualPayment
{
    public class ManualPaymentCustomerDetails
    {
        public string? BookingRef { get; set; }
        public decimal? Amount { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Postal { get; set; }
        public bool? PaymentStatus { get; set; }

    }
}
