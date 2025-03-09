using System;
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
        public string? AuthorizationCode { get; set; }
        public string? OrderID { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Acceptance { get; set; }
        public string? Status { get; set; }
        public string? CardNo { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CardNumber { get; set; }
        public string? TrxDate { get; set; }
        public string? PayId { get; set; }
        public string? NcError { get; set; }
        public string? Brand { get; set; }
        public string? Currency { get; set; }
        public string? IpCity { get; set; }
        public string? IP { get; set; }
    }
}
