﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Models
{
    public class FlightOffer
    {
        public string? type { get; set; }
        public string? id { get; set; }
        public string? source { get; set; }
        public bool? oneWay { get; set; }      
        public string? lastTicketingDate { get; set; }
        public List<Itinerary>? itineraries { get; set; }
        public Price? price { get; set; }
        public PriceOption? pricingOptions { get; set; }
        public List<string>? validatingAirlineCodes { get; set; }
        public List<TravelerPricing>? travelerPricings { get; set; }
        public string? cabinClass { get; set; }
        public string? bookingClass { get; set; }
        public string? avlStatus { get; set; }
        public string? fareBasis { get; set; }
        public string? passengerType { get; set; }
        public string? fareType { get; set; }
        public string? fareTypeName { get; set; }
        public string? fareTypeCode { get; set; }
        public string? breakPoint { get; set; }
        public BaggageDetails? baggageDetails { get; set; }
        public int? MarkupId { get; set; }
        public object? baggageXml { get; set; }
    }  

    public class FlightOfferForOrder
    {
        public string? type { get; set; }
        public string? id { get; set; }
        public string? source { get; set; }
        public bool? instantTicketingRequired { get; set; }
        public bool? nonHomogeneous { get; set; }
        public bool? paymentCardRequired { get; set; }
        public string? lastTicketingDate { get; set; }
        public List<Itinerary>? itineraries { get; set; }
        public Price? price { get; set; }
        public PriceOption? pricingOptions { get; set; }
        public List<string>? validatingAirlineCodes { get; set; }
        public List<TravelerPricing>? travelerPricings { get; set; }
    }

    public class BaggageDetails
    {
        public string? itemNumber { get; set; }
        public string? freeAllowance { get; set; }
        public string? quantityCode { get; set; }
        public string? unitQualifier { get; set; }
    }
}
