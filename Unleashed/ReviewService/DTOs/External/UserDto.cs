namespace ReviewService.DTOs.External
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? UserImage { get; set; }
    }
}
