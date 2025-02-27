using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.DB_Models
{
    [Table("deeplink")]
    public class Deeplink
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("deeplink_id")]
        public int DeeplinkId { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("country_name")]
        public string? CountryName { get; set; }

        [Column("city_name1")]
        public string? CityName1 { get; set; }

        [Column("price1")]
        public decimal? Price1 { get; set; }

        [Column("city_name2")]
        public string? CityName2 { get; set; }

        [Column("price2")]
        public decimal? Price2 { get; set; }

        [Column("city_name3")]
        public string? CityName3 { get; set; }

        [Column("price3")]
        public decimal? Price3 { get; set; }

        [Column("adults")]
        public int? Adults { get; set; }

        [Column("children")]
        public int? Children { get; set; }

        [Column("infant")]
        public int? Infant { get; set; }

        [Column("departuredate")]
        public string? DepartureDate{ get; set; }

        [Column("returndate")]
        public string? ReturnDate { get; set; }

        [Column("origin")]
        public string? Origin { get; set; }

        [Column("destination")]
        public string? Destination { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; }
        [Column("created_on")]
        public DateTime? CreatedOn { get; set; }

        [Column("cabin_class")]
        public string? CabinClass { get; set; }

        [Column("flight_type")]
        public string? FlightType { get; set; }

        [Column("adults2")]
        public int? Adults2 { get; set; }

        [Column("children2")]
        public int? Children2 { get; set; }

        [Column("infant2")]
        public int? Infant2 { get; set; }

        [Column("departuredate2")]
        public string? DepartureDate2 { get; set; }

        [Column("returndate2")]
        public string? ReturnDate2 { get; set; }

        [Column("origin2")]
        public string? Origin2 { get; set; }

        [Column("destination2")]
        public string? Destination2 { get; set; }

        [Column("cabin_class2")]
        public string? CabinClass2 { get; set; }

        [Column("flight_type2")]
        public string? FlightType2 { get; set; }

        [Column("adults3")]
        public int? Adults3 { get; set; }

        [Column("children3")]
        public int? Children3 { get; set; }

        [Column("infant3")]
        public int? Infant3 { get; set; }

        [Column("departuredate3")]
        public string? DepartureDate3 { get; set; }

        [Column("returndate3")]
        public string? ReturnDate3 { get; set; }

        [Column("origin3")]
        public string? Origin3 { get; set; }

        [Column("destination3")]
        public string? Destination3 { get; set; }

        [Column("cabin_class3")]
        public string? CabinClass3 { get; set; }

        [Column("flight_type3")]
        public string? FlightType3 { get; set; }
    }
}
