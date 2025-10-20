using ImgBBUploadService.Models;
using System.IO;
using System.Threading.Tasks;

public interface IImgBBUploadService
{
    /// <summary>
    /// Uploads an image from a byte array.
    /// </summary>
    /// <param name="imageBytes">The raw bytes of the image.</param>
    /// <param name="fileName">The desired file name for the upload (e.g., "image.jpg").</param>
    /// <param name="expirationInSeconds">Optional. Time in seconds to auto-delete the image (60 to 15552000).</param>
    /// <returns>The ImgBB API response.</returns>
    Task<ImgBBResponse> UploadAsync(byte[] imageBytes, string fileName, int? expirationInSeconds = null);

    /// <summary>
    /// Uploads an image from a Stream.
    /// </summary>
    /// <param name="imageStream">The stream containing the image data.</param>
    /// <param name="fileName">The desired file name for the upload (e.g., "image.jpg").</param>
    /// <param name="expirationInSeconds">Optional. Time in seconds to auto-delete the image (60 to 15552000).</param>
    /// <returns>The ImgBB API response.</returns>
    Task<ImgBBResponse> UploadAsync(Stream imageStream, string fileName, int? expirationInSeconds = null);

    /// <summary>
    /// Uploads an image from a base64 encoded string.
    /// </summary>
    /// <param name="base64Image">The base64 encoded image string.</param>
    /// <param name="fileName">The desired file name for the upload (e.g., "image.jpg").</param>
    /// <param name="expirationInSeconds">Optional. Time in seconds to auto-delete the image (60 to 15552000).</param>
    /// <returns>The ImgBB API response.</returns>
    Task<ImgBBResponse> UploadBase64Async(string base64Image, string fileName, int? expirationInSeconds = null);
}