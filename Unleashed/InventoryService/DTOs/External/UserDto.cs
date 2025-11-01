namespace InventoryService.DTOs.External
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string? UserUsername { get; set; }
        public string? UserEmail { get; set; }
    }
}
