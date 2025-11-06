namespace ReviewService.DTOs.External
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string? UserUsername { get; set; }
        public string? Email { get; set; }
        public string? UserImage { get; set; }
    }
}
