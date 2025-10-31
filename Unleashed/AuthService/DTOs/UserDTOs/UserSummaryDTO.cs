namespace AuthService.DTOs.UserDTOs
{
    public class UserSummaryDTO
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? UserImage { get; set; }
    }
}