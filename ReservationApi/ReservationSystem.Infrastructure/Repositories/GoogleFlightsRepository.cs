using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Repositories;
using ReservationSystem.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class GoogleFlightsRepository : IGoogleFlightsRepository
    {
        private ITravelBoardSearchRepository _availability;

        public GoogleFlightsRepository(ITravelBoardSearchRepository availability)
        {
            _availability = availability;            
        }
        public async Task<AvailabilityRequest> GetFlightRequeust(string requestModel)
        {

            var availabilityRequest = ParseFlightRequestXml(requestModel);
            return availabilityRequest;
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //sb.AppendLine("<FlightSearchRequest>");
            //sb.AppendLine("  <Origin>DXB</Origin>");
            //sb.AppendLine("  <Destination>LHR</Destination>");
            //sb.AppendLine("  <DepartureDate>2025-05-03</DepartureDate>");
            //sb.AppendLine("  <ReturnDate>2025-05-10</ReturnDate>");
            //sb.AppendLine("  <Adults>1</Adults>");
            //sb.AppendLine("  <Children>0</Children>");
            //sb.AppendLine("  <Infant>0</Infant>");
            //sb.AppendLine("  <CabinClass>y</CabinClass>");
            //sb.AppendLine("  <FlightType>c</FlightType>");
            //sb.AppendLine("  <IncludeAirlines></IncludeAirlines>");
            //sb.AppendLine("  <ExcludeAirlines></ExcludeAirlines>");
            //sb.AppendLine("  <NonStop></NonStop>");
            //sb.AppendLine("  <MaxFlights>250</MaxFlights>");
            //sb.AppendLine("</FlightSearchRequest>");
            //string xmlRequest = sb.ToString();
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xmlRequest);


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
                flightType = root.Element("FlightType")?.Value
            };
        }
    }
}
