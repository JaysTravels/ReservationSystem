using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApi.ReservationSystem.Domain.DB_Models
{
    [Table("booking_info")]
    public class BookingInfo
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("auto_id")]
        public int AutoId { get; set; }

        [Column("pnr_number")]
        public string? PnrNumber { get; set; }

        [Column("first_name")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        public string? LastName { get; set; }

        [Column("total_amount")]
        public decimal? TotalAmount { get; set; }

        [Column("session_id")]
        public string? SessionId { get; set; }

        [Column("payment_status")]
        public string? PaymentStatus { get; set; }

        [Column("error")]
        public string? Error{ get; set; }

        [Column("created_on")]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        [Column("booking_ref")]
        public string? BookingRef { get; set; }

        [Column("sent_email")]
        public bool? SentEmail { get; set; }

        [Column("order_id")]
        public string? OrderID { get; set; }

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("acceptance")]
        public string? Acceptance { get; set; }

        [Column("barclays_status")]
        public string? BarclaysStatus { get; set; }

        [Column("card_number")]
        public string? CardNumber { get; set; }

        [Column("brand")]
        public string? Brand { get; set; }

        [Column("card_holder_name")]
        public string? CardHolderName { get; set; }

        [Column("expiry_date")]
        public string? ExpiryDate { get; set; }

        [Column("Ip")]
        public string? Ip { get; set; }

        [Column("authorization_code")]
        public string? AuthorizationCode { get; set; }


    }
}
