namespace AuthService.DTOs.AuthenDTOs
{
    public class LoginUserResponeDTO
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
