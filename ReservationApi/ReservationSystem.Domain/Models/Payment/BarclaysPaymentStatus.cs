namespace ReservationApi.ReservationSystem.Domain.Models.Payment
{
    public enum BarclaysPaymentStatus
    {
        
        Authorized = 5,        
        PaymentCaptured = 9,        
        IncompleteOrInvalid = 0,   
        CancelledByCustomer = 1,   
        AuthorizationDeclined = 2, 
        OrderStored = 4,           
        WaitingExternalResult = 40,
        WaitingForClientPayment = 41, 
        AuthorizedWaitingResult = 46,      
        AuthorizedAndVoided = 6, 
        PaymentDeleted = 7,   
        Refunded = 8,           
        RefundInProgress = 83   
    }
}
