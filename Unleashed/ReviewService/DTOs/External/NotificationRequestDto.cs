namespace ReviewService.DTOs.External
{
    public class NotificationRequestDto
    {
        public string? NotificationTitle { get; set; }
        public string? NotificationContent { get; set; }
        public List<string> UserIds { get; set; } = new();
    }
}
