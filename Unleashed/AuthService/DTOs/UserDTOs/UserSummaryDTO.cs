namespace AuthService.DTOs.UserDTOs
{
    public class UserSummaryDTO
    {
        public Guid UserId { get; set; }
        public string? UserUsername { get; set; }
        public string? Email { get; set; }
        public string? UserImage { get; set; }
    }
}