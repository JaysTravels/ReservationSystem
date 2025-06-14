﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using ReservationSystem.Domain.Service;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using ReservationSystem.Domain.Repositories;
using Org.BouncyCastle.Asn1.Ocsp;
using ReservationSystem.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using ReservationSystem.Domain.Models.Enquiry;
using Azure;
using Azure.Communication.Email;
using ReservationSystem.Domain.Models.ManualPayment;
using DocumentFormat.OpenXml.Wordprocessing;
using ReservationSystem.Domain.Models.Insurance;
using DocumentFormat.OpenXml.Math;
using ReservationApi.ReservationSystem.Domain.Models.Payment;
using ReservationSystem.Domain.Models.AddPnrMulti;
using ReservationSystem.Domain.DB_Models;
using Newtonsoft.Json;


namespace ReservationSystem.Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IDBRepository _dBRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly EmailClient _emailClient;
        public EmailService(IConfiguration configuration , IDBRepository dBRepository, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _dBRepository = dBRepository;
            _environment = environment;
            string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
            if(connectionString == null)
            {
                connectionString = "endpoint=https://communicationservicesforjaystravels.uk.communication.azure.com/;accesskey=2cdbICCaEeMIXDjgVNMIYmliL76T9hEe9d2Y98pEpNL7lTy0OqUzJQQJ99BAACULyCpm8clbAAAAAZCSDuOI";
            }
            _emailClient = new EmailClient(connectionString);
        }

        public async Task SendEmailAsync3(string toEmail, string subject, string message)
        {
            try
            {
                string SenderName = "DoNotReply"; // Environment.GetEnvironmentVariable("EmailSettingsSenderName");
                string SenderEmail = "DoNotReply@jaystravels.co.uk"; //Environment.GetEnvironmentVariable("EmailSettingsSenderEmail");
                var bccRecipients = new List<string> ();
                List<EmailAddress> _bcc = new List<EmailAddress>();
                try
                {
                    if (_configuration["EmailSettings:BCCEmailAddress"] != null)
                    {
                        string[] cc = _configuration["EmailSettings:BCCEmailAddress"].Split(",");
                       foreach(var c in cc)
                        {
                            _bcc.Add(new EmailAddress(c));
                        }
                    }
                }
                catch
                {

                }
                var emailRecipients = new EmailRecipients(
                       to: new List<EmailAddress> { new EmailAddress(toEmail) },
                       bcc: _bcc
                   );
                var emailMessage = new EmailMessage(
                    senderAddress: SenderEmail,
                    recipients: emailRecipients,
                    content: new EmailContent(subject)
                    {
                        PlainText = subject,
                        Html = message
                    }
                );                
                EmailSendOperation emailSendOperation = _emailClient.Send(
                    WaitUntil.Completed,
                    emailMessage);               
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error while sending email {ex.StackTrace.ToString()}");
            }

        }

        public async Task SendEmailAsync2(string toEmail, string subject, string body)
        {
            string SmtpServer = Environment.GetEnvironmentVariable("EmailSettingsServer");
            string SenderName = Environment.GetEnvironmentVariable("EmailSettingsSenderName");
            string SenderEmail = Environment.GetEnvironmentVariable("EmailSettingsSenderEmail");
            string SmtpUser = Environment.GetEnvironmentVariable("EmailSettingsSmtpUser");
            string SmtpPass = Environment.GetEnvironmentVariable("EmailSettingsSmtpPass");
            string SmtpPort = Environment.GetEnvironmentVariable("EmailSettingsSmtpPort");            
            try {
                var smtpClient = new System.Net.Mail.SmtpClient(SmtpServer)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(
                        SmtpUser,
                        SmtpPass
                    ),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(SmtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);
                smtpClient.Send(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
             }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var configuration = _configuration.GetSection("EmailSettings");
                var _smtpServer = _configuration["EmailSettings:SmtpServer"];
                var _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var _smtpUser = _configuration["EmailSettings:SmtpUser"];
                var _smtpPass = _configuration["EmailSettings:SmtpPass"];
                using (var client = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUser),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public string GenerateFlightConfirmationEmail(string template, Dictionary<string, string> data)
        {
            foreach (var placeholder in data)
            {
                template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }
            return template;
        }

        public async Task<string> GetBookingSuccessTemplate(string sessionId = "", string bookingStatus = "" , string paymentStatus = "")
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "FlightConfirmation.html");
                var template = File.ReadAllText(filePath);
            
                if (sessionId != "")
                {
                    var flightInfo = await _dBRepository.GetFlightInfo(sessionId);
                    var passengerInfo = await _dBRepository.GetPassengerInfo(sessionId);
                    var bookingInfo = await _dBRepository.GetBookingInfo(sessionId);
                    FlightOffer offer = System.Text.Json.JsonSerializer.Deserialize<FlightOffer>(flightInfo?.FlightOffer);

                    var placeholders = new Dictionary<string, string>{
                  { "CustomerName", passengerInfo.Where(e=>e.IsLead == true).FirstOrDefault()?.FirstName + " " + passengerInfo.Where(e=>e.IsLead == true).FirstOrDefault()?.LastName  }, };


                    StringBuilder bookingref = new StringBuilder();
                    bookingref.Append($"<div class='segment'>");
                    bookingref.Append($"<table>");
                    bookingref.Append($"<tr><th>Booking Reference:</th><td>{bookingInfo.BookingRef}</td></tr>");
                    bookingref.Append($"</table>");
                    bookingref.Append($"</div>");
                    
                    template = template.Replace("{{BookingRef}}", bookingref.ToString());
                    template = template.Replace("{{currentyear}}", DateTime.Now.Year.ToString());
                    var segmentHtml = new StringBuilder(); 
                   
                    foreach (var item in offer.itineraries)
                    {
                        foreach (var segment in item.segments)
                        {
                            segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Flight Number:</th><td>{segment?.marketingCarrierCode}-{segment?.number}</td></tr>
                    <tr><th>Departure:</th><td>{segment?.departure?.iataName} ({segment?.departure?.iataCode}) at {segment?.departure?.at?.ToString()}</td></tr>
                    <tr><th>Arrival:</th><td>{segment?.arrival?.iataName?.ToString()} ({segment?.arrival?.iataCode?.ToString()}) at {segment?.arrival?.at?.ToString()}</td></tr>
                    <tr><th>Date:</th><td>{segment?.departure?.at?.ToString()}</td></tr>
                </table>
            </div>");
                        }                       
                    }
                    // Replace the placeholder with the actual segments
                    template = template.Replace("{{FlightSegments}}", segmentHtml.ToString());

                    StringBuilder pBuilder = new StringBuilder();
                   foreach( var p in passengerInfo)
                    {
                        pBuilder.Append($"<div class='segment'>");
                        pBuilder.Append($"<table>");
                        pBuilder.Append($"<tr><th>Passenger Name:</th><td>{p.FirstName + " " + p.LastName}</td></tr>");
                        pBuilder.Append($"<tr><th>Passenger Type:</th><td>{p.PassengerType}</td></tr>");
                        pBuilder.Append($"<tr><th>Date of Birth:</th><td>{p.DOB}</td></tr>");
                        pBuilder.Append($"</table>");
                        pBuilder.Append($"</div>");
                    }
                    template = template.Replace("{{PassengerList}}", pBuilder.ToString());
                    //placeholders.TryAdd("PassengerList", pBuilder.ToString());
                    placeholders.TryAdd("BookingStatus", bookingStatus);
                    placeholders.TryAdd("PaymentStatus", paymentStatus);
                    placeholders.TryAdd("FlightPrice",  offer?.price?.currency +" " + offer?.price?.total);
                    var emailBody = GenerateFlightConfirmationEmail(template, placeholders);                    
                    return emailBody;
                }
                else
                {
                    return "Error in sending email";
                }

              
              

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error While Sending Success booking email {ex.Message.ToString()}");
                return "Flights Booking Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }

        public async Task<string> GetEnquiryTemplate(EnquiryRequest enquiry)
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "Enquiry.html");
                var template = File.ReadAllText(filePath);
                var placeholders = new Dictionary<string, string>{
                  { "CustomerName", enquiry.FirstName + " " + enquiry.LastName  },};
                var segmentHtml = new StringBuilder();
                    segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Name:</th><td>{enquiry.FirstName + " " + enquiry.LastName}</td></tr>
                    <tr><th>Email:</th><td>{enquiry.EmailAddress}</td></tr>
                    <tr><th>Phone:</th><td>{enquiry.PhoneNumber}</td></tr>
                    <tr><th>Enquriy Messege:</th><td>{enquiry.Message}</td></tr>
                </table>
            </div>");

                    // Replace the placeholder with the actual segments
                template = template.Replace("{{Enquiry}}", segmentHtml.ToString());
                template = template.Replace("{{currentyear}}", DateTime.Now.Year.ToString());
                var emailBody = GenerateFlightConfirmationEmail(template, placeholders);
                return emailBody;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Sending Enquiry email {ex.Message.ToString()}");
                return "Enquiry Email Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }

        public async Task<string> GetBookingSuccessTemplateForAdmin(UpdatePaymentStatus payment, string bookingStatus = "")
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "AdminEmailFlightConfirmation.html");
                var template = File.ReadAllText(filePath);
                string paymentStatus = payment?.PaymentStatus;
                if (payment?.SessionId != "")
                {
                    string paymentMessage = "No Payment Status Available";
                    try
                    {
                        paymentMessage = payment?.Status != null
                            ? (BarclaysPaymentStatus)Convert.ToInt16(payment?.Status) switch
                            {
                                BarclaysPaymentStatus.Authorized => "Payment Authorized",
                                BarclaysPaymentStatus.PaymentCaptured => "Payment Captured",
                                BarclaysPaymentStatus.Refunded => "Payment Refunded",
                                BarclaysPaymentStatus.RefundInProgress => "Refund in Progress",
                                BarclaysPaymentStatus.AuthorizationDeclined => "Payment Declined",
                                BarclaysPaymentStatus.CancelledByCustomer => "Payment Cancelled",
                                _ => "Unknown Payment Status"
                            }
                            : "No Payment Status Available";
                    }
                    catch { }
                    var flightInfo = await _dBRepository.GetFlightInfo(payment.SessionId);
                    var passengerInfo = await _dBRepository.GetPassengerInfo(payment.SessionId);
                    var bookingInfo = await _dBRepository.GetBookingInfo(payment.SessionId);
                    if(flightInfo == null)
                    {
                        return "Error Flight object is null for SessionId " + payment?.SessionId;
                    }
                    
                    FlightOffer offer = System.Text.Json.JsonSerializer.Deserialize<FlightOffer>(flightInfo?.FlightOffer);

                    var bookingNo = bookingInfo?.BookingRef;
                    var bookingDate = bookingInfo?.CreatedOn.Value.ToString("dd-MM-yyyy");
                    var lastTicketDate = offer.lastTicketingDate;
                    var placeholders = new Dictionary<string, string>{
                  { "BookingNo", bookingNo  }};
                    placeholders.Add("BookingDate", bookingDate);
                    placeholders.Add("TiicketDeadLine", lastTicketDate);
                    placeholders.Add("PnrNumber", bookingInfo?.PnrNumber);

                    var segmentHtml = new StringBuilder();

                    foreach (var item in offer.itineraries)
                    {
                        foreach (var segment in item.segments)
                        {
                            segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Flight Number:</th><td>{segment?.marketingCarrierCode}-{segment?.number}</td></tr>
                    <tr><th>Departure:</th><td>{segment?.departure?.iataName} ({segment?.departure?.iataCode}) at {segment?.departure?.at?.ToString()}</td></tr>
                    <tr><th>Arrival:</th><td>{segment?.arrival?.iataName?.ToString()} ({segment?.arrival?.iataCode?.ToString()}) at {segment?.arrival?.at?.ToString()}</td></tr>
                    <tr><th>Date:</th><td>{segment?.departure?.at?.ToString()}</td></tr>
                </table>
            </div>");
                        }
                    }
                    // Replace the placeholder with the actual segments
                    template = template.Replace("{{FlightSegments}}", segmentHtml.ToString());

                    #region Price List
                    var priceList = new StringBuilder();
                    priceList.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Fare Type:</th><td>{offer?.fareType}</td></tr>
                    <tr><th>Adult Price:</th><td>{offer?.price?.adultPP}</td></tr>
                    <tr><th>Adult Tax:</th><td>{offer?.price?.adultTax}</td></tr>
                    <tr><th>Adult Markup:</th><td>{offer?.price?.adulMarkup}</td></tr>
                    <tr><th>Child Price:</th><td>{offer?.price?.childPp}</td></tr>
                    <tr><th>Child Tax:</th><td>{offer?.price?.childTax}</td></tr>
                    <tr><th>Child Markup:</th><td>{offer?.price?.childMarkup}</td></tr>
                    <tr><th>Infant Price:</th><td>{offer?.price?.infantPp}</td></tr>
                    <tr><th>Infant Tax:</th><td>{offer?.price?.infantTax}</td></tr>
                    <tr><th>Infant Markup:</th><td>{offer?.price?.infantMarkup}</td></tr>
                    <tr><th>Total Price:</th><td>{offer?.price?.total}</td></tr>
                </table>
            </div>");
                    template = template.Replace("{{PriceList}}", priceList.ToString());
                    #endregion

                    #region Fare Info
                    var fareInfo = new StringBuilder();
                    fareInfo.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Fare Type:</th><td>{offer?.fareType}</td></tr>
                    <tr><th>Fare Name:</th><td>{offer?.fareTypeName}</td></tr>
                    <tr><th>Fare Basis:</th><td>{offer?.fareBasis}</td></tr>
                    <tr><th>Booking Class:</th><td>{offer?.bookingClass}</td></tr>
                   
                </table>
            </div>");
                    template = template.Replace("{{FareInformation}}", fareInfo.ToString());
                    #endregion

                    #region payment details

                    var segmentHtmlPayResponse = new StringBuilder();
                    segmentHtmlPayResponse.Append($@"
<div class='segment'>
     <table>
        <tr><th>Payment Details:</th><td></td></tr>
        <tr><th>Authorization Code:</th><td>{payment?.Acceptance}</td></tr>
        <tr><th>Authorization Date:</th><td>{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}</td></tr>
        <tr><th>Payment Date:</th><td>{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}</td></tr>
        <tr><th>Currency:</th><td>{payment?.Currency}</td></tr>
        <tr><th>Amount:</th><td>{bookingInfo?.TotalAmount}</td></tr>
        <tr><th>Pay Id:</th><td>{payment?.PayId}</td></tr>
        <tr><th>Order ID:</th><td>{payment?.OrderID}</td></tr>
        <tr><th>Payment Method:</th><td>{payment?.PaymentMethod}</td></tr>                    
        <tr><th>Status:</th><td>{paymentMessage}</td></tr>
        <tr><th>Card No:</th><td>{payment?.CardNo}</td></tr>
        <tr><th>Brand:</th><td>{payment?.Brand}</td></tr>
        <tr><th>Card Holder Name:</th><td>{payment?.CardHolderName}</td></tr>
        <tr><th>Expiry Date:</th><td>{payment?.ExpiryDate}</td></tr>
        <tr><th>Error:</th><td>{payment?.NcError}</td></tr>
        <tr><th>IpCity:</th><td>{payment?.IpCity}</td></tr>
        <tr><th>Ip:</th><td>{payment?.IP}</td></tr>
      
    </table>
</div>");
                    template = template.Replace("{{CardDetails}}", segmentHtmlPayResponse.ToString());
                    #endregion

                    #region Search Details
                    var searcDetails = new StringBuilder();
                    if(offer?.itineraries?.Count > 1) // For Return Way
                    {
                        searcDetails.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>From Location:</th><td>{offer?.itineraries?[0].airport_city}</td></tr>
                    <tr><th>To Location:</th><td>{offer?.itineraries?[1]?.airport_city}</td></tr>
                    <tr><th>Departure:</th><td>{offer?.itineraries[0]?.segments[0]?.departure?.at}</td></tr>
                    <tr><th>Arrival:</th><td>{offer?.itineraries[1]?.segments[0]?.departure?.at}</td></tr>
                    <tr><th>No of Passengers:</th><td> {passengerInfo.Where(e => e.PassengerType == "ADT").Count()} Adults , {passengerInfo.Where(e => e.PassengerType == "CHD").Count()} Child , {passengerInfo.Where(e => e.PassengerType == "INF").Count()} Infants</td></tr>
                </table>
            </div>");
                    }
                    else  // For One Way
                    {
                        int? segCount = offer?.itineraries?[0].segments?.Count();
                        searcDetails.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>From Location:</th><td>{offer?.itineraries?[0]?.segments[0]?.departure?.iataCode + " " + offer?.itineraries?[0]?.segments[0]?.departure?.iataName}</td></tr>
                    <tr><th>To Location:</th><td>{offer?.itineraries?[0]?.segments[segCount.Value-1]?.arrival?.iataCode + " " + offer?.itineraries?[0]?.segments[segCount.Value-1]?.arrival?.iataName}</td></tr>
                    <tr><th>Departure:</th><td>{offer?.itineraries[0]?.segments[0]?.departure?.at}</td></tr>
                    <tr><th>Arrival:</th><td>{offer?.itineraries[0]?.segments[segCount.Value-1]?.arrival?.at}</td></tr>
                    <tr><th>No of Passengers:</th><td> {passengerInfo.Where(e => e.PassengerType == "ADT").Count()} Adults , {passengerInfo.Where(e => e.PassengerType == "CHD").Count()} Child , {passengerInfo.Where(e => e.PassengerType == "INF").Count()} Infants</td></tr>
                </table>
            </div>");
                    }


                        // Replace the placeholder with the actual segments
                        template = template.Replace("{{SearchDetails}}", searcDetails.ToString());
                    #endregion

                    StringBuilder pBuilder = new StringBuilder();
                    foreach (var p in passengerInfo)
                    {
                        pBuilder.Append($"<div class='segment'>");
                        pBuilder.Append($"<table>");
                        pBuilder.Append($"<tr><th>Passenger Name:</th><td>{p.FirstName + " " + p.LastName}</td></tr>");
                        pBuilder.Append($"<tr><th>Passenger Type:</th><td>{p.PassengerType}</td></tr>");
                        pBuilder.Append($"<tr><th>Date of Birth:</th><td>{p.DOB}</td></tr>");
                        pBuilder.Append($"</table>");
                        pBuilder.Append($"</div>");
                    }
                    template = template.Replace("{{PassengerInformation}}", pBuilder.ToString());
                    template = template.Replace("{{currentyear}}", DateTime.Now.Year.ToString());
                    bookingStatus = bookingInfo?.PnrNumber != null ? "Success" : "Failed";
                    placeholders.TryAdd("BookingStatus", bookingStatus);
                    placeholders.TryAdd("PaymentStatus",  paymentStatus);
                   // placeholders.TryAdd("HotelInformation", "<div class='segment'><span>None</span></div>");
                   // placeholders.TryAdd("TransferInformation", "<div class='segment'><span>None</span></div>");
                  //  placeholders.TryAdd("TourInformation", "<div class='segment'><span>None</span></div>");
                   // placeholders.TryAdd("CarInformation", "<div class='segment'><span>None</span></div>");
                   // placeholders.TryAdd("InsuranceInformation", "<div class='segment'><span>None</span></div>");
                   // placeholders.TryAdd("ParkingInformation", "<div class='segment'><span>None</span></div>");
                   // placeholders.TryAdd("PassengerList", "<div class='segment'><span>None</span></div>");
                  //  placeholders.TryAdd("FareInformation", fareInfo);

                    placeholders.TryAdd("FlightPrice", offer?.price?.currency + " " + offer?.price?.total);
                    var emailBody = GenerateFlightConfirmationEmail(template, placeholders);
                    return emailBody;
                }
                else
                {
                    return "Error in sending email";
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Sending Success booking email {ex.Message.ToString()}");
                return "Flights Booking Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }

        public async Task<string> GetManualPaymentTemplate(ManualPaymentCustomerDetails enquiry)
        {
            try
            {
                string templateName = "ManualPayment.html";
                if(enquiry.PaymentStatus == false)
                {
                    templateName = "ManualPaymentFailed.html";
                }
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", templateName);
                var template = File.ReadAllText(filePath);
                var placeholders = new Dictionary<string, string>{
                  { "CustomerName", enquiry.FirstName + " " + enquiry.LastName  },                  
                };
                placeholders.Add("PaymentStatus", enquiry?.PaymentStatus.ToString() == "True" ? "Success" : "Faild");
                var segmentHtml = new StringBuilder();
                segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Booking Ref:</th><td>{enquiry.BookingRef}</td></tr>
                    <tr><th>Amount:</th><td>{enquiry.Amount}</td></tr>
                    <tr><th>Name:</th><td>{enquiry.FirstName + " " + enquiry.LastName}</td></tr>
                    <tr><th>Email:</th><td>{enquiry.Email}</td></tr>
                    <tr><th>Phone:</th><td>{enquiry.Phone}</td></tr>
                    <tr><th>Address:</th><td>{enquiry.Address}</td></tr>
                    <tr><th>City:</th><td>{enquiry.City}</td></tr>
                    <tr><th>Country:</th><td>{enquiry.Country}</td></tr>
                    <tr><th>Postal:</th><td>{enquiry.Postal}</td></tr>
                </table>
            </div>");

                // Replace the placeholder with the actual segments
                template = template.Replace("{{PaymentDetails}}", segmentHtml.ToString());
                template = template.Replace("{{currentyear}}", DateTime.Now.Year.ToString());
                var emailBody = GenerateFlightConfirmationEmail(template, placeholders);
                return emailBody;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Sending Enquiry email {ex.Message.ToString()}");
                return "Enquiry Email Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }

        public async Task<string> GetManualPaymentTemplateAdmin(ManualPaymentCustomerDetails enquiry)
        {
            try
            {
                string paymentMessage = "No Payment Status Available";
                try
                {
                    paymentMessage = enquiry?.Status != null
                        ? (BarclaysPaymentStatus)Convert.ToInt16(enquiry.Status) switch
                        {
                            BarclaysPaymentStatus.Authorized => "Payment Authorized",
                            BarclaysPaymentStatus.PaymentCaptured => "Payment Captured",
                            BarclaysPaymentStatus.Refunded => "Payment Refunded",
                            BarclaysPaymentStatus.RefundInProgress => "Refund in Progress",
                            BarclaysPaymentStatus.AuthorizationDeclined => "Payment Declined",
                            BarclaysPaymentStatus.CancelledByCustomer => "Payment Cancelled",
                            _ => "Unknown Payment Status"
                        }
                        : "No Payment Status Available";
                }
                catch { }
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "ManualPaymentAdmin.html");
                var template = File.ReadAllText(filePath);
                var placeholders = new Dictionary<string, string>{
                  { "CustomerName", enquiry.FirstName + " " + enquiry.LastName  },
                };
                placeholders.Add("PaymentStatus", enquiry?.PaymentStatus.ToString() == "True" ? "Success" : "Faild");
                var paymentMsg = new StringBuilder();
                if (enquiry?.PaymentStatus == true)
                {
                    paymentMsg.Append($@"<p>Thank you for paying! Payment status: Success. We have received your payment with the details below:</p>");
                }
                else
                {
                    paymentMsg.Append($@"<p> Thank you for your payment! However, we have not received it. Payment status: Failed.</p>");
                }

                    var segmentHtml = new StringBuilder();
                segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Booking Ref:</th><td>{enquiry.BookingRef}</td></tr>
                    <tr><th>Amount:</th><td>{enquiry.Amount}</td></tr>
                    <tr><th>Name:</th><td>{enquiry.FirstName + " " + enquiry.LastName}</td></tr>
                    <tr><th>Email:</th><td>{enquiry.Email}</td></tr>
                    <tr><th>Phone:</th><td>{enquiry.Phone}</td></tr>
                    <tr><th>Address:</th><td>{enquiry.Address}</td></tr>
                    <tr><th>City:</th><td>{enquiry.City}</td></tr>
                    <tr><th>Country:</th><td>{enquiry.Country}</td></tr>
                    <tr><th>Postal:</th><td>{enquiry.Postal}</td></tr>
                </table>
            </div>");

                var segmentHtmlPayResponse = new StringBuilder();
                segmentHtmlPayResponse.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Payment Details:</th><td></td></tr>
                    <tr><th>Authorization Code:</th><td>{enquiry?.Acceptance}</td></tr>
                    <tr><th>Authorization Date:</th><td>{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}</td></tr>
                    <tr><th>Payment Date:</th><td>{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}</td></tr>
                    <tr><th>Currency:</th><td>{enquiry?.Currency}</td></tr>
                    <tr><th>Amount:</th><td>{enquiry?.Amount}</td></tr>
                    <tr><th>Pay Id:</th><td>{enquiry?.PayId}</td></tr>
                    <tr><th>Order ID:</th><td>{enquiry?.OrderID}</td></tr>
                    <tr><th>Payment Method:</th><td>{enquiry?.PaymentMethod}</td></tr>                    
                    <tr><th>Status:</th><td>{ paymentMessage}</td></tr>
                    <tr><th>Card No:</th><td>{enquiry?.CardNo}</td></tr>
                    <tr><th>Brand:</th><td>{enquiry?.Brand}</td></tr>
                    <tr><th>Card Holder Name:</th><td>{enquiry?.CardHolderName}</td></tr>
                    <tr><th>Expiry Date:</th><td>{enquiry?.ExpiryDate}</td></tr>
                    <tr><th>Error:</th><td>{enquiry?.NcError}</td></tr>
                    <tr><th>IpCity:</th><td>{enquiry?.IpCity}</td></tr>
                    <tr><th>Ip:</th><td>{enquiry?.IP}</td></tr>
                  
                </table>
            </div>");

                // Replace the placeholder with the actual segments
                template = template.Replace("{{PaymentDetails}}", segmentHtml.ToString());
                template = template.Replace("{{CardDetails}}", segmentHtmlPayResponse.ToString());
                template = template.Replace("{{PaymentMessage}}", paymentMsg.ToString());
                template = template.Replace("{{currentyear}}", DateTime.Now.Year.ToString());
                var emailBody = GenerateFlightConfirmationEmail(template, placeholders);
                return emailBody;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Sending Enquiry email {ex.Message.ToString()}");
                return "Enquiry Email Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }


        public async Task<string> GetPassengerSelectedFlightTemplate(List<PassengerInfo> passengerInfo,string sessionId = "", string selectedFlight = "")
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "PassengerSelectedFlight.html");
                var template = File.ReadAllText(filePath);

                if (sessionId != "")
                {
          
                    //var bookingInfo = await _dBRepository.GetBookingInfo(sessionId);
                    FlightOffer offer = new FlightOffer();
                    offer = System.Text.Json.JsonSerializer.Deserialize<FlightOffer>(selectedFlight);
                   var placeholders = new Dictionary<string, string>{
                  { "CustomerName", passengerInfo.Where(e=>e.IsLead == true).FirstOrDefault()?.FirstName + " " + passengerInfo.Where(e=>e.IsLead == true).FirstOrDefault()?.LastName  }, };

                    var segmentHtml = new StringBuilder();
                    string FlightPrice = offer.price?.total;
                    string AdultMarkup = offer.price?.adulMarkup != null ?  offer.price?.adulMarkup : "No Markp Applied";
                    string ChildMarkup = offer.price?.childMarkup != null ? offer.price?.childMarkup : "No Markp Applied";
                    string InfantMarkup = offer.price.infantMarkup != null ? offer.price?.infantMarkup : "No Markup Applied";
                    string FlightMarkupID = offer.price?.MarkupID != null ? offer.price?.MarkupID?.ToString() : "";
                    foreach (var item in offer.itineraries)
                    {
                        foreach (var segment in item.segments)
                        {
                            segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Flight Number:</th><td>{segment?.marketingCarrierCode}-{segment?.number}</td></tr>
                    <tr><th>Departure:</th><td>{segment?.departure?.iataName} ({segment?.departure?.iataCode}) at {segment?.departure?.at?.ToString()}</td></tr>
                    <tr><th>Arrival:</th><td>{segment?.arrival?.iataName?.ToString()} ({segment?.arrival?.iataCode?.ToString()}) at {segment?.arrival?.at?.ToString()}</td></tr>
                    <tr><th>Date:</th><td>{segment?.departure?.at?.ToString()}</td></tr>
                </table>
            </div>");
                        }
                    }
                    // Replace the placeholder with the actual segments
                    template = template.Replace("{{FlightSegments}}", segmentHtml.ToString());
                    template = template.Replace("{{FlightPrice}}", FlightPrice?.ToString());
                    template = template.Replace("{{AdultMarkup}}", AdultMarkup?.ToString());
                    template = template.Replace("{{ChildMarkup}}", ChildMarkup?.ToString());
                    template = template.Replace("{{InfantMarkup}}", InfantMarkup?.ToString());
                    template = template.Replace("{{FlightMarkupID}}", FlightMarkupID?.ToString());

                    StringBuilder pBuilder = new StringBuilder();
                    foreach (var p in passengerInfo)
                    {
                        pBuilder.Append($"<div class='segment'>");
                        pBuilder.Append($"<table>");
                        pBuilder.Append($"<tr><th>Passenger Name:</th><td>{p.FirstName + " " + p.LastName}</td></tr>");
                        pBuilder.Append($"<tr><th>Passenger Type:</th><td>{p.PassengerType}</td></tr>");
                        pBuilder.Append($"<tr><th>Date of Birth:</th><td>{p.DOB}</td></tr>");
                        try
                        {
                            if (!String.IsNullOrEmpty(p.PhoneNumber))
                            {
                                pBuilder.Append($"<tr><th>Contact:</th><td>{p.PhoneNumber}</td></tr>");
                            }
                            if (!String.IsNullOrEmpty(p.Email))
                            {
                                pBuilder.Append($"<tr><th>Email:</th><td>{p.Email}</td></tr>");
                            }
                        }
                        catch { }
                        pBuilder.Append($"</table>");
                        pBuilder.Append($"</div>");
                    }
                    
                    template = template.Replace("{{PassengerList}}", pBuilder.ToString());
                    template = template.Replace("{{currentyear}}", DateTime.Now.Year.ToString());
                    placeholders.TryAdd("FlightPrice", offer?.price?.currency + " " + offer?.price?.total);
                    var emailBody = GenerateFlightConfirmationEmail(template, placeholders);
                    return emailBody;
                }
                else
                {
                    return "Error in sending email";
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Sending Success booking email {ex.Message.ToString()}");
                return "Flights Booking Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }

        public async Task<string> GetInsuranceTemplate(InsuranceRequest request)
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "Insurance.html");
                var template = File.ReadAllText(filePath);
                var placeholders = new Dictionary<string, string>{
                  { "Customer", "Customer"  },{ "currentyear" , DateTime.Now.Year.ToString() } };
                var segmentHtml = new StringBuilder();
                segmentHtml.Append($@"
            <div class='segment'>
                 <table>
                    <tr><th>Where To:</th><td>{request?.WhereTo}</td></tr>
                    <tr><th>Number of Travellers:</th><td>{request?.NumnerOfTravellers}</td></tr>
                    <tr><th>Departure Date:</th><td>{request?.DepartureDate.ToString("dd-MM-yyyy")}</td></tr>
                    <tr><th>Return Date:</th><td>{request?.ReturnDate.ToString("dd-MM-yyyy")}</td></tr>
                    <tr><th>Email:</th><td>{request?.Email}</td></tr>
                    <tr><th>Contact:</th><td>{request?.Contact}</td></tr>
                </table>
            </div>");

                // Replace the placeholder with the actual segments
                template = template.Replace("{{Insurance}}", segmentHtml.ToString());
                var emailBody = GenerateFlightConfirmationEmail(template, placeholders);
                return emailBody;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Sending Enquiry email {ex.Message.ToString()}");
                return "Enquiry Email Error in sending Email " + ex.Message.ToString() + " " + ex.StackTrace.ToString();
            }
        }
    }
}
