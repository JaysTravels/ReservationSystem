﻿using ReservationSystem.Domain.Models.Soap.FlightPrice;
using ReservationSystem.Domain.Models;
using ReservationSystem.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSystem.Domain.Models.AirSellFromRecommendation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography;
using ReservationSystem.Domain.Models.Availability;
using ReservationSystem.Domain.Models.FlightPrice;
using System.Globalization;
using ReservationSystem.Domain.Models.AddPnrMulti;
using ReservationSystem.Domain.Service;
using System.Data;
using ReservationSystem.Domain.Models.DBLogs;

namespace ReservationSystem.Infrastructure.Repositories
{
    public class AirsellRepository : IAirSellRepository
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private readonly IHelperRepository _helperRepository;
        private readonly ICacheService _cacheService;
        private readonly IDBRepository _dbRepository;
        public AirsellRepository(IConfiguration _configuration, IMemoryCache cache, IHelperRepository helperRepository, ICacheService cacheService, IDBRepository dBRepository)
        {
            configuration = _configuration;
            _cache = cache;
            _helperRepository = helperRepository;
            _cacheService = cacheService;
            _dbRepository = dBRepository;

        }

        public async Task<AirSellFromRecResponseModel> GetAirSellRecommendation(AirSellFromRecommendationRequest requestModel)
        {
            AirSellFromRecResponseModel AirSell = new AirSellFromRecResponseModel();

            try
            {
                #region Temp Region for read response from xml file
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "SupportFiles", "AirSell_Response.xml");
                //var xDocumentTemp = XDocument.Load(filePath);
                //var restemp = ConvertXmlToModel(xDocumentTemp);
                //AirSell = restemp;
                //return AirSell;
                #endregion
                var _url = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:ApiUrl"]);
                var _action = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:Air_SellFromRecommendation"]);
                string Result = string.Empty;
                string Envelope = await CreateAirSellRecommendationRequest(requestModel);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
                request.Headers.Add("SOAPAction", _action);
                request.ContentType = "text/xml;charset=\"utf-8\"";
                request.Accept = "text/xml";
                request.Method = "POST";

                using (Stream stream = request.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(Envelope);
                    stream.Write(content, 0, content.Length);
                }

                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        {
                            var result2 = rd.ReadToEnd();
                            XDocument xmlDoc = XDocument.Parse(result2);
                            await _helperRepository.SaveXmlResponse("AirSell_Request", Envelope);
                            await _helperRepository.SaveXmlResponse("AirSell_Response", result2);


                            XmlDocument xmlDoc2 = new XmlDocument();
                            xmlDoc2.LoadXml(result2);

                            string jsonText = JsonConvert.SerializeXmlNode(xmlDoc2, Newtonsoft.Json.Formatting.Indented);
                            await _helperRepository.SaveJson(jsonText, "AirSellResponseJson");
                            XNamespace fareNS = "http://xml.amadeus.com/ITARES_05_2_IA";
                            SaveReservationLog saveReservationLog = new SaveReservationLog();
                            saveReservationLog.Request = Envelope;
                            saveReservationLog.Response = jsonText;
                            saveReservationLog.RequestName = RequestName.AirSell.ToString();
                            saveReservationLog.UserId = 0;
                            var errorInfo = xmlDoc.Descendants(fareNS + "errorAtMessageLevel").FirstOrDefault();
                            if (errorInfo != null)
                            {
                                // Extract error details
                                var errorCode = errorInfo.Descendants(fareNS + "errorSegment").Descendants(fareNS + "errorDetails").Descendants(fareNS + "errorCode").FirstOrDefault()?.Value;
                                var errorText = errorInfo.Descendants(fareNS + "errorSegment").Descendants(fareNS + "errorDetails").Descendants(fareNS + "errorCategory").FirstOrDefault()?.Value;
                                AirSell.amadeusError = new AmadeusResponseError();
                                AirSell.amadeusError.error = errorText;
                                AirSell.amadeusError.errorCode = Convert.ToInt16(errorCode);
                                #region DB Logs
                                try
                                {
                                    saveReservationLog.IsError = true;
                                    saveReservationLog.AmadeusSessionId = "";
                                    await _dbRepository.SaveReservationFlow(saveReservationLog);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error while save db logs {ex.Message.ToString()}");
                                }
                                #endregion
                                return AirSell;

                            }
                            var res = ConvertXmlToModel(xmlDoc);
                            AirSell = res;
                            #region DB Logs
                            try
                            {
                                saveReservationLog.IsError = false;
                                saveReservationLog.AmadeusSessionId = res?.session?.SessionId;
                                await _dbRepository.SaveReservationFlow(saveReservationLog);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while save db logs {ex.Message.ToString()}");
                            }
                            #endregion



                        }
                    }
                }
                catch (WebException ex)
                {
                    using (StreamReader rd = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        Result = rd.ReadToEnd();
                        AirSell.amadeusError = new AmadeusResponseError();
                        AirSell.amadeusError.error = Result;
                        AirSell.amadeusError.errorCode = 0;
                        return AirSell;

                    }
                }
            }
            catch (Exception ex)
            {
                AirSell.amadeusError = new AmadeusResponseError();
                AirSell.amadeusError.error = ex.Message.ToString();
                AirSell.amadeusError.errorCode = 0;
                return AirSell;
            }
            return AirSell;
        }
        public AirSellFromRecResponseModel ConvertXmlToModel(XDocument response)
        {
            AirSellFromRecResponseModel ReturnModel = new AirSellFromRecResponseModel();
            ReturnModel.airSellResponse = new List<AirSellItineraryDetails>();
            XDocument doc = response;

            XNamespace awsse = "http://xml.amadeus.com/2010/06/Session_v3";
            XNamespace wsa = "http://www.w3.org/2005/08/addressing";

            var AirlineCache = _cacheService.GetAirlines();
            var AirportCache = _cacheService.GetAirports();

            var sessionElement = doc.Descendants(awsse + "Session").FirstOrDefault();
            if (sessionElement != null)
            {

                string sessionId = sessionElement?.Element(awsse + "SessionId")?.Value;
                string sequenceNumber = sessionElement?.Element(awsse + "SequenceNumber")?.Value;
                string securityToken = sessionElement?.Element(awsse + "SecurityToken")?.Value;
                string TransactionStatusCode = sessionElement?.Attribute("TransactionStatusCode")?.Value;
                int SeqNumber = 0;
                if (sequenceNumber != null) { SeqNumber = Convert.ToInt32(sequenceNumber); }
                ReturnModel.session = new HeaderSession
                {
                    SecurityToken = securityToken,
                    SequenceNumber = SeqNumber,
                    SessionId = sessionId,
                    TransactionStatusCode = TransactionStatusCode
                };
            }

            XNamespace amadeus = "http://xml.amadeus.com/ITARES_05_2_IA";
            var messegeFunction = doc.Descendants(amadeus + "message")?.Descendants(amadeus + "messageFunctionDetails")?.Descendants(amadeus + "messageFunction")?.FirstOrDefault()?.Value;
            var itineraryDetails = doc.Descendants(amadeus + "itineraryDetails")?.ToList();
            if (itineraryDetails != null)
            {
                foreach (var item in itineraryDetails)
                {
                    AirSellItineraryDetails airSellItinerary = new AirSellItineraryDetails();
                    airSellItinerary.messageFunction = messegeFunction;
                    var origin = item?.Descendants(amadeus + "originDestination")?.Elements(amadeus + "origin")?.FirstOrDefault()?.Value;
                    var destination = item?.Descendants(amadeus + "originDestination")?.Elements(amadeus + "destination")?.FirstOrDefault()?.Value;
                    airSellItinerary.originDestination = new OriginDestination { origin = origin, destination = destination };
                    List<AirSellFlightDetails> LstflightDetails = new List<AirSellFlightDetails>();
                    var segmentInformation = item?.Descendants(amadeus + "segmentInformation")?.ToList();
                    foreach (var item2 in segmentInformation)
                    {
                        AirSellFlightDetails flightDetails = new AirSellFlightDetails();
                        var departureDate = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "departureDate")?.FirstOrDefault()?.Value;
                        if (departureDate != null)
                        {
                            try
                            {
                                DateTime deptdate = DateTime.ParseExact(departureDate, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                                flightDetails.departureDate = DateOnly.FromDateTime(deptdate);
                            }
                            catch { }

                        }
                        var departureTime = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "departureTime")?.FirstOrDefault().Value;
                        if (departureTime != null)
                        {
                            try
                            {
                                TimeOnly deptTime = TimeOnly.ParseExact(departureTime, "HHmm");
                                flightDetails.departureTime = deptTime;
                            }
                            catch { }

                        }
                        var arrivalDate = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "arrivalDate")?.FirstOrDefault()?.Value;
                        if (arrivalDate != null)
                        {
                            try
                            {
                                DateTime date = DateTime.ParseExact(arrivalDate, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                                flightDetails.arrivalDate = DateOnly.FromDateTime(date);
                            }
                            catch
                            {

                            }

                        }
                        var arrivalTime = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "arrivalTime")?.FirstOrDefault()?.Value;
                        if (arrivalTime != null)
                        {
                            try
                            {
                                TimeOnly arrTime = TimeOnly.ParseExact(arrivalTime, "HHmm");
                                flightDetails.arrivalTime = arrTime;
                            }
                            catch { }

                        }

                        var fromAirport = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "boardPointDetails")?.Elements(amadeus + "trueLocationId")?.FirstOrDefault()?.Value;
                        var toAirport = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "offpointDetails")?.Elements(amadeus + "trueLocationId")?.FirstOrDefault()?.Value;
                        flightDetails.fromAirport = fromAirport;
                        DataRow fromAirportName = AirportCache != null ? AirportCache?.AsEnumerable()?.FirstOrDefault(r => r.Field<string>("AirportCode") == fromAirport) : null;
                        var depAirportName = fromAirportName != null ? fromAirportName[2]?.ToString() + " , " + fromAirportName[4]?.ToString() : "";
                        flightDetails.fromAirportName = depAirportName;

                        flightDetails.toAirport = toAirport;
                        DataRow toAirportName = AirportCache != null ? AirportCache?.AsEnumerable()?.FirstOrDefault(r => r.Field<string>("AirportCode") == toAirport) : null;
                        var arrAirportName = toAirportName != null ? toAirportName[2]?.ToString() + " , " + toAirportName[4]?.ToString() : "";
                        flightDetails.toAirportName = arrAirportName;

                        var marketingCompany = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "companyDetails")?.Elements(amadeus + "marketingCompany")?.FirstOrDefault()?.Value;
                        flightDetails.marketingCompany = marketingCompany;
                        var flightNumber = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightIdentification")?.Elements(amadeus + "flightNumber")?.FirstOrDefault()?.Value;
                        var bookingClass = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightIdentification")?.Elements(amadeus + "bookingClass")?.FirstOrDefault()?.Value;
                        flightDetails.flightNumber = flightNumber;
                        flightDetails.marketingCompany = marketingCompany;
                        flightDetails.bookingClass = bookingClass;
                        DataRow carrier = AirlineCache != null ? AirlineCache?.AsEnumerable()?.FirstOrDefault(r => r.Field<string>("AirlineCode") == marketingCompany) : null;
                        var carriername = carrier != null ? carrier[1]?.ToString() : "";
                        flightDetails.marketingCompanyName = carriername;
                        var flightIndicator = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightTypeDetails")?.Elements(amadeus + "flightIndicator")?.FirstOrDefault()?.Value;
                        flightDetails.flightIndicator = flightIndicator;
                        var specialSegment = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "specialSegment")?.FirstOrDefault()?.Value;
                        flightDetails.specialSegment = specialSegment;
                        var equipment = item2?.Descendants(amadeus + "apdSegment")?.Descendants(amadeus + "legDetails")?.Elements(amadeus + "equipment")?.FirstOrDefault()?.Value;
                        flightDetails.legdetails = new LegDetails { equipment = equipment };
                        var deptTerminal = item2?.Descendants(amadeus + "apdSegment")?.Descendants(amadeus + "departureStationInfo")?.Elements(amadeus + "terminal")?.FirstOrDefault()?.Value;
                        var arrivalTerminal = item2?.Descendants(amadeus + "apdSegment")?.Descendants(amadeus + "arrivalStationInfo")?.Elements(amadeus + "terminal")?.FirstOrDefault()?.Value;
                        flightDetails.departureTerminal = deptTerminal;
                        flightDetails.arrivalTerminal = arrivalTerminal;
                        var statusCode = item2?.Descendants(amadeus + "actionDetails")?.Elements(amadeus + "statusCode")?.FirstOrDefault()?.Value;
                        flightDetails.statusCode = statusCode;
                        LstflightDetails.Add(flightDetails);
                    }

                    airSellItinerary.flightDetails = LstflightDetails;
                    ReturnModel.airSellResponse.Add(airSellItinerary);

                }
            }
            return ReturnModel;
        }
        private string getSegmentInfoOutbound(AirSellFromRecommendationRequest model)
        {
            string info = @"";
            foreach (var det in model.outBound.segmentInformation.travelProductInformation)
            {
                info = info + @" <segmentInformation>
               <travelProductInformation>
                  <flightDate>
                     <departureDate>" + det.departureDate + @"</departureDate>
                  </flightDate>
                  <boardPointDetails>
                     <trueLocationId>" + det.fromAirport + @"</trueLocationId>
                  </boardPointDetails>
                  <offpointDetails>
                     <trueLocationId>" + det.toAirport + @"</trueLocationId>
                  </offpointDetails>
                  <companyDetails>
                     <marketingCompany>" + det.marketingCompany + @"</marketingCompany>
                  </companyDetails>
                  <flightIdentification>
                     <flightNumber>" + det.flightNumber + @"</flightNumber>
                     <bookingClass>" + det.bookingClass + @"</bookingClass>
                  </flightIdentification>
               </travelProductInformation>
               <relatedproductInformation>
                  <quantity>" + det.relatedproductInformation.quantity + @"</quantity>
                  <statusCode>" + det.relatedproductInformation.statusCode + @"</statusCode>
               </relatedproductInformation>
            </segmentInformation>";
            }

            return info;
        }

        private string getSegmentInfoInbound(AirSellFromRecommendationRequest model)
        {
            string info = @"";

            foreach (var det in model.inBound.segmentInformation.travelProductInformation)
            {
                info = info + @"<segmentInformation>
               <travelProductInformation>
                  <flightDate>
                     <departureDate>" + det.departureDate + @"</departureDate>
                  </flightDate>
                  <boardPointDetails>
                     <trueLocationId>" + det.fromAirport + @"</trueLocationId>
                  </boardPointDetails>
                  <offpointDetails>
                     <trueLocationId>" + det.toAirport + @"</trueLocationId>
                  </offpointDetails>
                  <companyDetails>
                     <marketingCompany>" + det.marketingCompany + @"</marketingCompany>
                  </companyDetails>
                  <flightIdentification>
                     <flightNumber>" + det.flightNumber + @"</flightNumber>
                     <bookingClass>" + det.bookingClass + @"</bookingClass>
                  </flightIdentification>
               </travelProductInformation>
               <relatedproductInformation>
                  <quantity>" + det.relatedproductInformation.quantity + @"</quantity>
                  <statusCode>" + det.relatedproductInformation.statusCode + @"</statusCode>
               </relatedproductInformation>
            </segmentInformation>";
            }

            return info;
        }
        public async Task<string> CreateAirSellRecommendationRequest(AirSellFromRecommendationRequest requestModel)
        {
            string pwdDigest = await _helperRepository.generatePassword();
            // var amadeusSettings = configuration.GetSection("AmadeusSoap") != null ? configuration.GetSection("AmadeusSoap") : null;
            string action = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:Air_SellFromRecommendation"]);
            string to = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:ApiUrl"]);
            string username = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:webUserId"]);
            string dutyCode = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:dutyCode"]);
            string requesterType = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:requestorType"]);
            string PseudoCityCode = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:PseudoCityCode"]?.ToString());
            string pos_type = Environment.GetEnvironmentVariable(configuration["AmadeusSoap:POS_Type"]);

            // string Request = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:sec=""http://xml.amadeus.com/2010/06/Security_v1"" xmlns:typ=""http://xml.amadeus.com/2010/06/Types_v1"" xmlns:iat=""http://www.iata.org/IATA/2007/00/IATA2010.1"" xmlns:app=""http://xml.amadeus.com/2010/06/AppMdw_CommonTypes_v3"" xmlns:link=""http://wsdl.amadeus.com/2010/06/ws/Link_v1"" xmlns:ses=""http://xml.amadeus.com/2010/06/Session_v3"">
            string Request = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ses=""http://xml.amadeus.com/2010/06/Session_v3"">
       <soap:Header xmlns:add=""http://www.w3.org/2005/08/addressing"">
     <ses:Session TransactionStatusCode=""Start""/>
     <add:MessageID>{System.Guid.NewGuid()}</add:MessageID>
     <add:Action>{action}</add:Action>
     <add:To>{to}</add:To>
     <link:TransactionFlowLink xmlns:link=""http://wsdl.amadeus.com/2010/06/ws/Link_v1""/>
     <oas:Security xmlns:oas=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:oas1=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
        <oas:UsernameToken oas1:Id=""UsernameToken-1"">
           <oas:Username>{username}</oas:Username>
           <oas:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">{pwdDigest.Split("|")[1]}</oas:Nonce>
           <oas:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest"">{pwdDigest.Split("|")[0]}</oas:Password>
           <oas1:Created>{pwdDigest.Split("|")[2]}</oas1:Created>
        </oas:UsernameToken>
     </oas:Security>
     <AMA_SecurityHostedUser xmlns=""http://xml.amadeus.com/2010/06/Security_v1"">
        <UserID AgentDutyCode=""{dutyCode}"" RequestorType=""{requesterType}"" PseudoCityCode=""{PseudoCityCode}"" POS_Type=""{pos_type}""/>
     </AMA_SecurityHostedUser>
  </soap:Header>
   <soap:Body>
      <Air_SellFromRecommendation>
         <messageActionDetails>
            <messageFunctionDetails>
               <messageFunction>{requestModel.messageFunction}</messageFunction>
               <additionalMessageFunction>{requestModel.additionalMessageFunction}</additionalMessageFunction>
            </messageFunctionDetails>
         </messageActionDetails>
         <itineraryDetails>
            <originDestinationDetails>
               <origin>{requestModel.outBound.origin}</origin>
               <destination>{requestModel.outBound.destination}</destination>
            </originDestinationDetails>
            <message>
               <messageFunctionDetails>
                  <messageFunction>{requestModel.messageFunction}</messageFunction>
               </messageFunctionDetails>
            </message>
            {getSegmentInfoOutbound(requestModel)}
         </itineraryDetails>";
            if(requestModel.inBound != null)
            {
                Request = Request + @"<itineraryDetails>
            <originDestinationDetails>
               <origin>"+requestModel?.inBound?.origin+@"</origin>
               <destination>"+requestModel?.inBound?.destination+@"</destination>
            </originDestinationDetails>
            <message>
               <messageFunctionDetails>
                  <messageFunction>"+requestModel?.messageFunction+@"</messageFunction>
               </messageFunctionDetails>
            </message>
            " +getSegmentInfoInbound(requestModel)+@"
         </itineraryDetails>";
            }
           Request = Request + 
      @"</Air_SellFromRecommendation>
   </soap:Body>

</soap:Envelope>";

            return Request;
        }


    }
}