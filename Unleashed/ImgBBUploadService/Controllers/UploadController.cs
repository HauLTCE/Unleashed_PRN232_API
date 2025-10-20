using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    private readonly IImgBBUploadService _uploadService;

    public UploadController(IImgBBUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost("file")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Copy file data to a memory stream
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        // Reset stream position
        memoryStream.Position = 0;

        // Use the service to upload
        // Set an expiration of 10 minutes (600 seconds)
        var response = await _uploadService.UploadAsync(memoryStream, file.FileName, expirationInSeconds: 600);

        if (response.Success)
        {
            // Return the direct image URL
            return Ok(new { imageUrl = response.Data.DisplayUrl });
        }

        return StatusCode((int)response.Status, response.Error.Message);
    }

    [HttpPost("base64")]
    public async Task<IActionResult> UploadBase64Image([FromBody] Base64UploadRequest request)
    {
        if (string.IsNullOrEmpty(request.Base64Image))
        {
            return BadRequest("Base64 string is missing.");
        }

        // The base64 string might include a data prefix like "data:image/png;base64,"
        // We must remove it before sending to ImgBB.
        var base64Data = request.Base64Image.Split(',').Last();

        var response = await _uploadService.UploadBase64Async(base64Data, request.FileName);

        if (response.Success)
        {
            return Ok(new { imageUrl = response.Data.DisplayUrl });
        }

        return StatusCode((int)response.Status, response.Error.Message);
    }
}

// A simple model for the base64 request
public class Base64UploadRequest
{
    public string Base64Image { get; set; }
    public string FileName { get; set; } = "upload.jpg"; // Default filename
}