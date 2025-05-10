using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ReservationSystem.Domain.Models;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class GoogleFlightsRepository : IGoogleFlightsRepository
    {
        private ITravelBoardSearchRepository _availability;
        private readonly ICacheService _cacheService;
        private readonly IDBRepository _dbRepository;

        public GoogleFlightsRepository(ITravelBoardSearchRepository availability, ICacheService cacheService, IDBRepository dbRepository)
        {
            _availability = availability;
            _cacheService = cacheService;
            _dbRepository = dbRepository;
        }
        public async Task<AvailabilityRequest> GetFlightRequeust(string requestModel)
        {
            var availabilityRequest = ParseFlightRequestXml(requestModel);
            return availabilityRequest;
        }

        public static AvailabilityRequest ParseFlightRequestXml(string xmlString)
        {
            var doc = XDocument.Parse(xmlString);
            var root = doc.Element("FlightSearchRequest");

            return new AvailabilityRequest
            {
                origin = root.Element("Origin")?.Value,
                destination = root.Element("Destination")?.Value,
                departureDate = root.Element("DepartureDate")?.Value,
                returnDate = root.Element("ReturnDate")?.Value,
                adults = int.Parse(root.Element("Adults")?.Value ?? "0"),
                children = int.Parse(root.Element("Children")?.Value ?? "0"),
                infant = int.Parse(root.Element("Infant")?.Value ?? "0"),
                cabinClass = root.Element("CabinClass")?.Value?.ToUpper(),
                flightType = root.Element("FlightType")?.Value,
                oneWay = root.Element("oneWay")?.Value != null ? Convert.ToBoolean(root.Element("oneWay")?.Value) : false,
                maxFlights = 250,

            };
        }


        public async Task<string> CreateXmlFeed(AvailabilityRequest objsearch , AvailabilityModel punitflights)
        { 
            StringBuilder sbresponse = new StringBuilder();
            try
            {
                string journey = "";
                string? cabin = objsearch.cabinClass;
                int? childcount = objsearch?.children + objsearch?.infant;
                int? intNoOfPassenger = objsearch?.adults + objsearch?.children + objsearch?.infant;
               

                if (objsearch?.oneWay == true)
                    journey = "O";
                else
                    journey = "R";

                try
                {
                    string sAges = "";
                    if (objsearch?.children > 0)
                    {
                        for (int iChild = 0; iChild < objsearch.children; iChild++)
                        {
                            sAges += "3,";
                        }
                    }
                    if (objsearch?.infant > 0)
                    {
                        for (int iInfant = 0; iInfant < objsearch.infant; iInfant++)
                        {
                            sAges += "1,";
                        }
                    }
                    if (childcount.HasValue && childcount.Value >0)
                    {
                        int iLoopEnd = (4 - childcount.Value);
                        for (int iFinal = 0; iFinal < (4 - childcount); iFinal++)
                        {
                            iLoopEnd -= 1;

                            if (iLoopEnd == 0)
                            {
                                sAges += "0";
                            }
                            else
                            {
                                sAges += "0,";
                            }
                        }
                    }
                  

                    sbresponse.Append("<FlightJourneyAvailabilityResponse>");

                    for (int loop = 0; loop < punitflights?.data?.Count(); loop++)
                    {
                         StringBuilder deeplink = new StringBuilder();

                        deeplink.Append("https://jaystravels.co.uk/waitgoogle?");
                        deeplink.Append("google_redirectid=" + System.Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "&");
                        deeplink.Append("DeparturingFrom=" + objsearch?.origin + "&");
                        deeplink.Append("Goingto=" + objsearch?.destination + "&");
                        deeplink.Append("DeparturingDate=" + objsearch?.departureDate + "&");
                        deeplink.Append("ReturnDate=" + objsearch?.returnDate + "&");
                        deeplink.Append("JourneyType=" + objsearch?.oneWay != null && objsearch?.oneWay == true ? "OneWay" : "Return" + "&");
                        deeplink.Append("DirectFlight=False&");
                        deeplink.Append("CabinClass="+cabin+"&");                       
                        deeplink.Append("adult=" + objsearch?.adults + "&");
                        deeplink.Append("child=" + objsearch?.children + "&");
                        deeplink.Append("totPassenger=" + intNoOfPassenger.Value.ToString() + "&");
                        deeplink.Append("Airline=&");
                        //string json = JsonSerializer.Serialize(punitflights?.data?[loop]);
                        //string base64Flight = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
                        string flightDictKey =  System.Guid.NewGuid().ToString();
                        _cacheService.Set(flightDictKey, punitflights?.data?[loop], TimeSpan.FromMinutes(60));
                        //if (childcount > 0)
                            deeplink.Append("ChildAges=" + sAges + "&");
                        //else
                        //    deeplink.Append("ChildAges=0,0,0,0&");
                        deeplink.Append("Infanttype=0&");
                        deeplink.Append("flight="+ flightDictKey);
                        string? dLPrice = punitflights?.data?[loop]?.price?.total;    // This price logic due to First Position on Sky
                        deeplink.Append("&price=" + dLPrice + "&");
                        deeplink.Append("airline=" + punitflights?.data?[loop]?.itineraries?[0]?.segments?[0]?.marketingCarrierCode + "&");
                        sbresponse.Append("<Flight CabinClass=" + "\"" + cabin + "\"" + ">");
                        sbresponse.Append("<Itinerary JourneyType=" + "\"" + journey + "\"" + ">");


                        string? tempAirlinecode = punitflights?.data?[loop]?.itineraries?[0]?.segments?[0]?.marketingCarrierCode;
                        string? tempAirlineName = punitflights?.data?[loop]?.itineraries?[0]?.segments?[0]?.marketingCarrierName;

                        sbresponse.Append("<OutboundLegs>");
                        for (int legsLoop = 0; legsLoop < punitflights?.data?[loop]?.itineraries?[0]?.segments?.Count(); legsLoop++)
                        {
                            sbresponse.Append("<Leg id=" + "\"" + (legsLoop + 1) + "\"" + ">");
                            sbresponse.Append(" <DepartureAirportCode>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.departure?.iataCode  + "</DepartureAirportCode> ");
                            sbresponse.Append(" <DepartureAirportName>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.departure?.iataName + "</DepartureAirportName> ");
                            sbresponse.Append(" <DestinationAirportCode>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.arrival?.iataCode + "</DestinationAirportCode> ");
                            sbresponse.Append(" <DestinationAirportName>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.arrival?.iataName + "</DestinationAirportName>");
             
                            string[]? deptdatetime = punitflights?.data?[loop]?.itineraries?[0]?.segments?[legsLoop]?.departure?.at?.ToString().Split(' ');
                            deptdatetime[0] = deptdatetime[0].Replace('/', '-');
                            string[] deptdatepart = deptdatetime[0].Split('-');
                            sbresponse.Append(" <DepartureDate>" + deptdatepart[2] + "-" + deptdatepart[1] + "-" + deptdatepart[0] + "</DepartureDate> ");
                            string[] deptime = deptdatetime[1].Split(':');
                            sbresponse.Append(" <DepartureTime>" + deptime[0] + ":" + deptime[1] + "</DepartureTime> ");

                            string[]? arrdatetime = punitflights?.data?[loop]?.itineraries?[0]?.segments?[legsLoop]?.arrival?.at?.ToString().Split(' ');
                            arrdatetime[0] = arrdatetime[0].Replace('/', '-');
                            string[] arrdatepart = arrdatetime[0].Split('-');
                            sbresponse.Append(" <ArrivalDate>" + arrdatepart[2] + "-" + arrdatepart[1] + "-" + arrdatepart[0] + "</ArrivalDate> ");
                            string[] arrtime = arrdatetime[1].Split(':');
                            sbresponse.Append(" <ArrivalTime>" + arrtime[0] + ":" + arrtime[1] + "</ArrivalTime>");
                            sbresponse.Append(" <AirlineCode>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.marketingCarrierCode + "</AirlineCode>");
                            sbresponse.Append(" <AirlineName>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.marketingCarrierName + "</AirlineName>");
                            sbresponse.Append(" <FlightNumber>" + punitflights?.data?[loop]?.itineraries?[0].segments?[legsLoop]?.aircraft?.code + "</FlightNumber>");
                            sbresponse.Append("</Leg>");
                        }

                        #region pricing
                        sbresponse.Append("<PriceDetails>");
                        sbresponse.Append("<Currency>GBP</Currency>");
                        sbresponse.Append("<PriceBreakdown>");

                        if (objsearch?.adults > 0)
                        {
                            sbresponse.Append("<Passenger Type=\"Adult\">");
                            sbresponse.Append("<Quantity>" + objsearch.adults + "</Quantity>");
                            decimal bPObAPrice = Convert.ToDecimal(punitflights?.data[loop]?.price?.adultPP);
                            sbresponse.Append("<BasePrice>" + bPObAPrice + "</BasePrice>");
                            decimal tPObAPrice = (decimal)(Convert.ToDouble(punitflights?.data[loop]?.price?.adultTax) );
                            sbresponse.Append("<Tax>" + tPObAPrice + "</Tax>");
                            sbresponse.Append("</Passenger>");

                        }

                        if (objsearch?.children > 0)
                        {
                            sbresponse.Append("<Passenger Type=\"Child\">");
                            sbresponse.Append("<Quantity>" + objsearch?.children + "</Quantity>");
                            sbresponse.Append("<BasePrice>" + punitflights?.data[loop]?.price?.childPp + "</BasePrice>");
                            sbresponse.Append("<Tax>" + punitflights?.data[loop]?.price?.childTax + "</Tax>");
                            sbresponse.Append("</Passenger>");

                        }

                        if (objsearch?.infant > 0)
                        {
                            sbresponse.Append("<Passenger Type=\"Infant\">");
                            sbresponse.Append("<Quantity>" + objsearch.infant + "</Quantity>");
                            sbresponse.Append("<BasePrice>" + punitflights?.data[loop]?.price?.infantPp + "</BasePrice>");
                            sbresponse.Append("<Tax>" + punitflights?.data[loop]?.price?.infantTax + "</Tax>");
                            sbresponse.Append("</Passenger>");
                        }
                        sbresponse.Append("</PriceBreakdown>");
                        sbresponse.Append("</PriceDetails>");
                        #endregion

                        sbresponse.Append("</OutboundLegs>");

                        if (journey == "R")
                        {
                            sbresponse.Append("<InboundLegs>");
                            for (int legsLoop = 0; legsLoop < punitflights?.data?[loop]?.itineraries?[1]?.segments?.Count(); legsLoop++)
                            {
                                sbresponse.Append("<Leg id=" + "\"" + (legsLoop + 1) + "\"" + ">");
                                sbresponse.Append(" <DepartureAirportCode>" + punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.departure?.iataCode + "</DepartureAirportCode> ");
                                sbresponse.Append(" <DepartureAirportName>" + punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.departure?.iataName + "</DepartureAirportName> ");
                                sbresponse.Append(" <DestinationAirportCode>" + punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.arrival?.iataCode + "</DestinationAirportCode> ");
                                sbresponse.Append(" <DestinationAirportName>" + punitflights?.data[loop]?.itineraries?[1]?.segments?[legsLoop]?.arrival?.iataName + "</DestinationAirportName>");

                                string[]? deptdatetime = punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.departure?.at?.ToString().Split(' ');
                                deptdatetime[0] = deptdatetime[0].Replace('/', '-');
                                string[] deptdatepart = deptdatetime[0].Split('-');
                                sbresponse.Append(" <DepartureDate>" + deptdatepart[2] + "-" + deptdatepart[1] + "-" + deptdatepart[0] + "</DepartureDate> ");
                                string[] deptime = deptdatetime[1].Split(':');
                                sbresponse.Append(" <DepartureTime>" + deptime[0] + ":" + deptime[1] + "</DepartureTime> ");

                                string[]? arrdatetime = punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.arrival?.at?.ToString().Split(' ');
                                if(arrdatetime != null)
                                {
                                    arrdatetime[0] = arrdatetime[0].Replace('/', '-');
                                }                                
                                string[]? arrdatepart = arrdatetime?[0]?.Split('-');
                                sbresponse.Append(" <ArrivalDate>" + arrdatepart?[2] + "-" + arrdatepart?[1] + "-" + arrdatepart?[0] + "</ArrivalDate> ");
                                string[]? arrtime = arrdatetime?[1].Split(':');
                                sbresponse.Append(" <ArrivalTime>" + arrtime?[0] + ":" + arrtime?[1] + "</ArrivalTime>");
                                sbresponse.Append(" <AirlineCode>" + punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.marketingCarrierCode + "</AirlineCode>");
                                sbresponse.Append(" <AirlineName>" + punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.marketingCarrierName + "</AirlineName>");
                                sbresponse.Append(" <FlightNumber>" + punitflights?.data?[loop]?.itineraries?[1]?.segments?[legsLoop]?.aircraft?.code + "</FlightNumber>");
                                sbresponse.Append("</Leg>");
                            }

                            #region pricing
                            sbresponse.Append("<PriceDetails>");
                            sbresponse.Append("<Currency>GBP</Currency>");
                            sbresponse.Append("<PriceBreakdown>");

                            if (objsearch?.adults > 0)
                            {
                                sbresponse.Append("<Passenger Type=\"Adult\">");
                                sbresponse.Append("<Quantity>" + objsearch?.adults + "</Quantity>");
                                decimal bPObAPrice = Convert.ToDecimal(punitflights?.data[loop]?.price?.adultPP);
                                sbresponse.Append("<BasePrice>" + bPObAPrice + "</BasePrice>");
                                decimal tPObAPrice = (decimal)(Convert.ToDouble(punitflights?.data[loop]?.price?.adultTax));
                                sbresponse.Append("<Tax>" + tPObAPrice + "</Tax>");
                                sbresponse.Append("</Passenger>");
                            }

                            if (objsearch?.children > 0)
                            {
                                sbresponse.Append("<Passenger Type=\"Child\">");
                                sbresponse.Append("<Quantity>" + objsearch?.children + "</Quantity>");
                                sbresponse.Append("<BasePrice>" + punitflights?.data[loop]?.price?.childPp + "</BasePrice>");
                                sbresponse.Append("<Tax>" + punitflights?.data[loop]?.price?.childTax + "</Tax>");
                                sbresponse.Append("</Passenger>");

                            }

                            if (objsearch?.infant > 0)
                            {
                                sbresponse.Append("<Passenger Type=\"Infant\">");
                                sbresponse.Append("<Quantity>" + objsearch.infant + "</Quantity>");
                                sbresponse.Append("<BasePrice>" + punitflights?.data[loop]?.price?.infantPp + "</BasePrice>");
                                sbresponse.Append("<Tax>" + punitflights?.data[loop]?.price?.infantTax + "</Tax>");
                                sbresponse.Append("</Passenger>");

                            }
                            sbresponse.Append("</PriceBreakdown>");
                            sbresponse.Append("</PriceDetails>");
                            #endregion

                            sbresponse.Append("</InboundLegs>");
                        }

                        sbresponse.Append("</Itinerary>");

                        sbresponse.Append("<DeepLink>");
                        sbresponse.Append("<![CDATA[" + deeplink.ToString() + "]]>");
                        sbresponse.Append("</DeepLink>");

                        sbresponse.Append("</Flight>");
                    }


                    sbresponse.Append("</FlightJourneyAvailabilityResponse>");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while generate xml { ex.Message.ToString()}");
                    sbresponse = new StringBuilder();
                    sbresponse.Append("<FlightJourneyAvailabilityResponse>");
                    sbresponse.Append("<Flight><Message>No Flights Found.</Message>");
                    sbresponse.Append("</Flight>");
                    sbresponse.Append("</FlightJourneyAvailabilityResponse>");
                }
                
            }
            catch (Exception ex)
            {
                return $"Error While Create Xml {ex.Message.ToString()}";
            }
            return sbresponse.ToString();
        }

        public async Task<FlightOffer?> GetFlightFromCache(string flightId)
        {
            try
            {
                var flightOffer = _cacheService.Get<FlightOffer>(flightId);
                return flightOffer;
            }
            catch
            {
                return null;
            }
        }
    }
}
