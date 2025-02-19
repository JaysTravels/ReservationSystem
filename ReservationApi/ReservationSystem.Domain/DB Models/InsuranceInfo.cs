using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.DB_Models
{
    [Table("insurance_info")]
    public class InsuranceInfo
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("insurance_id")]
        public int InsurancneId { get; set; }

        [Column("where_to")]
        public string? WhereTo { get; set; }

        [Column("number_of_travellers")]
        public string? NumberOfTravellers { get; set; }

        [Column("depture_date")]
        public DateTime? DepartureDate { get; set; }

        [Column("return_date")]
        public DateTime? ReturnDate { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("contact")]
        public string? Contact { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
