using System.Text.Json.Serialization;
namespace ImgBBUploadService.Models
{
    /// <summary>
    /// The main wrapper for the ImgBB API response.
    /// </summary>
    public class ImgBBResponse
    {
        [JsonPropertyName("data")]
        public required ImgBBData Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("error")]
        public ImgBBError? Error { get; set; }
    }

    /// <summary>
    /// Contains the data of the uploaded image.
    /// </summary>
    public class ImgBBData
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("url_viewer")]
        public string? UrlViewer { get; set; }

        // This is the direct link to the image
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        // This is the URL to use in <img> tags
        [JsonPropertyName("display_url")]
        public string? DisplayUrl { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("time")]
        public long? Time { get; set; }

        [JsonPropertyName("expiration")]
        public long? Expiration { get; set; }

        [JsonPropertyName("thumb")]
        public ImgBBImageInfo? Thumb { get; set; }

        [JsonPropertyName("medium")]
        public ImgBBImageInfo? Medium { get; set; }

        [JsonPropertyName("image")]
        public ImgBBImageInfo? Image { get; set; }
    }

    /// <summary>
    /// Contains URL and other info for a specific image variant (e.g., thumb, medium).
    /// </summary>
    public class ImgBBImageInfo
    {
        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("mime")]
        public string? Mime { get; set; }

        [JsonPropertyName("extension")]
        public string? Extension { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    /// <summary>
    /// Contains error details if the upload fails.
    /// </summary>
    public class ImgBBError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }
    }
}
