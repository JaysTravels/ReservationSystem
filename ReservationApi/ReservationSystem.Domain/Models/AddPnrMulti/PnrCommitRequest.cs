using ReservationSystem.Domain.DB_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.AddPnrMulti
{
    public class PnrCommitRequest
    {
        public HeaderSession sessionDetails { get; set; }
        public int? optionCode1 { get; set; }
        public int? optionCode2 { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BookingRef { get; set; }

    }

    public class UpdatePaymentStatus
    {
        public string? SessionId { get; set; }        
        public string? PaymentStatus { get; set; }
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
        public string? selectedFlightOffer { get; set; }
        public List<PassengerInfo>? passengerInfo { get; set; }

    }
}
