using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Insurance
{
    public class InsuranceRequest
    {
        [Required(ErrorMessage = "Destinationn is required.")]
        public string WhereTo { get; set; }

        [Required(ErrorMessage = "Number of Travellers are required.")]
        public string NumnerOfTravellers { get; set; }

        [Required(ErrorMessage = "Departure Date is required.")]
        public DateTime DepartureDate { get; set; }

        [Required(ErrorMessage = "Return Date is required.")]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^(?i)[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public string Contact { get; set; }
    }
   
}
