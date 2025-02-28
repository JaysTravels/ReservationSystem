using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models.Deeplink
{
    public class DeepLinkModel
    {
        [Key]
        [Display(Name = "Deeplink ID")]
        public int DeeplinkId { get; set; }

        [Column("image_url")]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Column("country_name")]
        [Display(Name = "Country Name")]
        public string? CountryName { get; set; }

        [Column("city_name1")]
        [Display(Name = "City Name 1")]
        public string? CityName1 { get; set; }

        [Column("price1")]
        [Display(Name = "Price 1")]
        public decimal? Price1 { get; set; }

        [Column("city_name2")]
        [Display(Name = "City Name 2")]
        public string? CityName2 { get; set; }

        [Column("price2")]
        [Display(Name = "Price 2")]
        public decimal? Price2 { get; set; }

        [Column("city_name3")]
        [Display(Name = "City Name 3")]
        public string? CityName3 { get; set; }

        [Column("price3")]
        [Display(Name = "Price 3")]
        public decimal? Price3 { get; set; }

        [Column("adults")]
        [Display(Name = "Adults")]
        public int? Adults { get; set; }

        [Column("children")]
        [Display(Name = "Children")]
        public int? Children { get; set; }

        [Column("infant")]
        [Display(Name = "Infant")]
        public int? Infant { get; set; }

        [Column("departuredate")]
        [Display(Name = "Departure Date")]
        public string? DepartureDate { get; set; }

        [Column("returndate")]
        [Display(Name = "Return Date")]
        public string? ReturnDate { get; set; }

        [Column("origin")]
        [Display(Name = "Origin")]
        public string? Origin { get; set; }

        [Column("destination")]
        [Display(Name = "Destination")]
        public string? Destination { get; set; }

        [Column("is_active")]
        [Display(Name = "Is Active")]
        public bool? IsActive { get; set; }

        [Column("created_on")]
        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }

        [Column("cabin_class")]
        [Display(Name = "Cabin Class")]
        public string? CabinClass { get; set; }

        [Column("flight_type")]
        [Display(Name = "Flight Type")]
        public string? FlightType { get; set; }

        [Column("adults2")]
        [Display(Name = "Adults 2")]
        public int? Adults2 { get; set; }

        [Column("children2")]
        [Display(Name = "Children 2")]
        public int? Children2 { get; set; }

        [Column("infant2")]
        [Display(Name = "Infant 2")]
        public int? Infant2 { get; set; }

        [Column("departuredate2")]
        [Display(Name = "Departure Date 2")]
        public string? DepartureDate2 { get; set; }

        [Column("returndate2")]
        [Display(Name = "Return Date 2")]
        public string? ReturnDate2 { get; set; }

        [Column("origin2")]
        [Display(Name = "Origin 2")]
        public string? Origin2 { get; set; }

        [Column("destination2")]
        [Display(Name = "Destination 2")]
        public string? Destination2 { get; set; }

        [Column("cabin_class2")]
        [Display(Name = "Cabin Class 2")]
        public string? CabinClass2 { get; set; }

        [Column("flight_type2")]
        [Display(Name = "Flight Type 2")]
        public string? FlightType2 { get; set; }

        [Column("adults3")]
        [Display(Name = "Adults 3")]
        public int? Adults3 { get; set; }

        [Column("children3")]
        [Display(Name = "Children 3")]
        public int? Children3 { get; set; }

        [Column("infant3")]
        [Display(Name = "Infant 3")]
        public int? Infant3 { get; set; }

        [Column("departuredate3")]
        [Display(Name = "Departure Date 3")]
        public string? DepartureDate3 { get; set; }

        [Column("returndate3")]
        [Display(Name = "Return Date 3")]
        public string? ReturnDate3 { get; set; }

        [Column("origin3")]
        [Display(Name = "Origin 3")]
        public string? Origin3 { get; set; }

        [Column("destination3")]
        [Display(Name = "Destination 3")]
        public string? Destination3 { get; set; }

        [Column("cabin_class3")]
        [Display(Name = "Cabin Class 3")]
        public string? CabinClass3 { get; set; }

        [Column("flight_type3")]
        [Display(Name = "Flight Type 3")]
        public string? FlightType3 { get; set; }
    }
}
