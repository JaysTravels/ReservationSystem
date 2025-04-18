﻿using ReservationSystem.Domain.DB_Models;
using ReservationSystem.Domain.Models.AddPnrMulti;
using ReservationSystem.Domain.Models.Enquiry;
using ReservationSystem.Domain.Models.Insurance;
using ReservationSystem.Domain.Models.ManualPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Service
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);

        public Task SendEmailAsync2(string toEmail, string subject, string body);

        public Task SendEmailAsync3(string toEmail, string subject, string message);      

        public Task<string> GetBookingSuccessTemplate(string sessionId = "", string bookingStatus = "", string paymentStatus = "");

        public Task<string> GetEnquiryTemplate(EnquiryRequest enquiry);

        public Task<string> GetInsuranceTemplate(InsuranceRequest request);

        public Task<string> GetBookingSuccessTemplateForAdmin(UpdatePaymentStatus payment, string bookingStatus = "");

        public Task<string> GetManualPaymentTemplate(ManualPaymentCustomerDetails enquiry);

        public Task<string> GetPassengerSelectedFlightTemplate(List<PassengerInfo> passengerInfo,string sessionId = "" , string selectedFlight="");

        public Task<string> GetManualPaymentTemplateAdmin(ManualPaymentCustomerDetails enquiry);
    }
}
