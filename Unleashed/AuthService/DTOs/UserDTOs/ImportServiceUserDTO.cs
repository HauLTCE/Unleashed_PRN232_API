namespace AuthService.DTOs.UserDTOs
{
    public class ImportServiceUserDTO
    {
        public Guid UserId { get; set; }
        public string? UserUsername { get; set; }
        public string? UserEmail { get; set; }
    }
}
