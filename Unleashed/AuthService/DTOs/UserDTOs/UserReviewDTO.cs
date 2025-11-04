namespace AuthService.DTOs.UserDTOs
{
    public class UserReviewDTO
    {
        public Guid UserId { get; set; }
        public string? UserFullname { get; set; }

        public string? UserImage { get; set; }
    }
}
