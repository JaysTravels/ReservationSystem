﻿using ReservationSystem.Domain.Models.Soap.FlightPrice;

           var AirlineCache = _cacheService.GetAirlines();
           var AirportCache = _cacheService.GetAirports();
                    {
                        AirSellFlightDetails flightDetails = new AirSellFlightDetails();
                        var departureDate = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "departureDate").FirstOrDefault().Value;
                        if (departureDate != null)
                        {
                            DateTime deptdate = DateTime.ParseExact(departureDate, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                            flightDetails.departureDate = DateOnly.FromDateTime(deptdate);
                        }
                        var departureTime = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "departureTime").FirstOrDefault().Value;
                        if (departureTime != null)
                        {
                            TimeOnly deptTime = TimeOnly.ParseExact(departureTime, "HHmm");
                            flightDetails.departureTime = deptTime;
                        }
                        var arrivalDate = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "arrivalDate").FirstOrDefault().Value;
                        if (arrivalDate != null)
                        {
                            DateTime date = DateTime.ParseExact(arrivalDate, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                            flightDetails.arrivalDate = DateOnly.FromDateTime(date);
                        }
                        var arrivalTime = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightDate")?.Elements(amadeus + "departureTime").FirstOrDefault().Value;
                        if (arrivalTime != null)
                        {
                            TimeOnly arrTime = TimeOnly.ParseExact(arrivalTime, "HHmm");
                            flightDetails.arrivalTime = arrTime;
                        }

                        var fromAirport = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "boardPointDetails")?.Elements(amadeus + "trueLocationId")?.FirstOrDefault()?.Value;
                        var toAirport = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "offpointDetails")?.Elements(amadeus + "trueLocationId")?.FirstOrDefault()?.Value;
                        flightDetails.fromAirport = fromAirport;
                        DataRow fromAirportName = AirportCache != null ? AirportCache.AsEnumerable().FirstOrDefault(r => r.Field<string>("AirportCode") == fromAirport): null;
                        var depAirportName = fromAirportName != null ? fromAirportName[2].ToString() + " , " + fromAirportName[4].ToString() : "";
                        flightDetails.fromAirportName = depAirportName;

                        flightDetails.toAirport = toAirport;
                        DataRow toAirportName = AirportCache != null ? AirportCache.AsEnumerable().FirstOrDefault(r => r.Field<string>("AirportCode") == toAirport) : null;
                        var arrAirportName = toAirportName != null ? toAirportName[2].ToString() + " , " + toAirportName[4].ToString() : "";
                        flightDetails.toAirportName = arrAirportName;

                        var marketingCompany = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "companyDetails")?.Elements(amadeus + "marketingCompany")?.FirstOrDefault()?.Value;
                        flightDetails.marketingCompany = marketingCompany;
                        var flightNumber = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightIdentification")?.Elements(amadeus + "flightNumber")?.FirstOrDefault()?.Value;
                        var bookingClass = item2?.Descendants(amadeus + "flightDetails")?.Descendants(amadeus + "flightIdentification")?.Elements(amadeus + "bookingClass")?.FirstOrDefault()?.Value;
                        flightDetails.flightNumber = flightNumber;
                        flightDetails.marketingCompany = marketingCompany;
                        flightDetails.bookingClass = bookingClass;
                        DataRow carrier = AirlineCache != null ? AirlineCache.AsEnumerable().FirstOrDefault(r => r.Field<string>("AirlineCode") == marketingCompany): null;
                        var carriername = carrier != null ? carrier[1].ToString() : "";
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