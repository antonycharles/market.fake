namespace User.Core.Responses
{
    public class UserPhotoResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentUrl { get; set; }
    }
}
