namespace AuthService.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public bool? IsUserEnabled { get; set; }
        public string? UserUsername { get; set; }
        public string? UserFullname { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? UserBirthdate { get; set; }
        public string? UserAddress { get; set; }
        public string? UserImage { get; set; }
        public DateTimeOffset? UserCreatedAt { get; set; }
    }
}
