namespace User.Core.Responses
{
    public class UserCreditCardResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string HolderName { get; set; }
        public string Brand { get; set; }
        public string LastFourDigits { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public bool IsPrimary { get; set; }
    }
}
