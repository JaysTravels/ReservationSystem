using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationApi.ReservationSystem.Domain.DB_Models
{
    [Table("manual_payment")]
    public class ManualPayment
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("first_name")]
        public string? FirstName { get; set; }

        [Column("last_name")]
        public string? LastName { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("city")]
        public string? City { get; set; }

        [Column("postal_code")]
        public string? PostalCode { get; set; }

        [Column("country")]
        public string? Country { get; set; }

        public DateTime? CreatedOn { get; set; }

        [Column("payment_status")]
        public bool? PaymentStatus { get; set; }

        [Column("booking_ref")]
        public string? BookingRef { get; set; }

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

        [Column("error")]
        public string? Error { get; set; }

        [Column("Ip")]
        public string? Ip { get; set; }
    }
}
