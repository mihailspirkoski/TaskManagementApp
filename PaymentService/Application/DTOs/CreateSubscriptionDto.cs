namespace PaymentService.Application.DTOs
{
    public class CreateSubscriptionDto
    {
        public string PaymentMethodId { get; set; }
        public string PriceId { get; set; }
    }

}
