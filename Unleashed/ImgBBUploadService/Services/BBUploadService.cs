using ImgBBUploadService.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// Helper class for configuration
public class ImgBBOptions
{
    public string ApiKey { get; set; }
}

public class BBUploadService : IImgBBUploadService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string UploadEndpoint = "https://api.imgbb.com/1/upload";

    public BBUploadService(HttpClient httpClient, IOptions<ImgBBOptions> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;

        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new ArgumentException("ImgBB API Key is not configured in appsettings.json.");
        }
    }

    /// <summary>
    /// Uploads an image from a byte array.
    /// </summary>
    public async Task<ImgBBResponse> UploadAsync(byte[] imageBytes, string fileName, int? expirationInSeconds = null)
    {
        using var stream = new MemoryStream(imageBytes);
        return await UploadAsync(stream, fileName, expirationInSeconds);
    }

    /// <summary>
    /// Uploads an image from a Stream. This is the core method.
    /// </summary>
    public async Task<ImgBBResponse> UploadAsync(Stream imageStream, string fileName, int? expirationInSeconds = null)
    {
        using var content = new MultipartFormDataContent();

        // 1. Add API Key
        content.Add(new StringContent(_apiKey), "key");

        // 2. Add Image Stream
        var streamContent = new StreamContent(imageStream);
        content.Add(streamContent, "image", fileName);

        // 3. (Optional) Add expiration
        if (expirationInSeconds.HasValue)
        {
            content.Add(new StringContent(expirationInSeconds.Value.ToString()), "expiration");
        }

        // 4. (Optional) Add name (this sets the title)
        content.Add(new StringContent(fileName), "name");

        // 5. Make the request
        var response = await _httpClient.PostAsync(UploadEndpoint, content);

        // 6. Process the response
        return await ProcessResponse(response);
    }

    /// <summary>
    /// Uploads an image from a base64 encoded string.
    /// </summary>
    public async Task<ImgBBResponse> UploadBase64Async(string base64Image, string fileName, int? expirationInSeconds = null)
    {
        // Use FormUrlEncodedContent for base64, as it's just a string field
        var parameters = new Dictionary<string, string>
        {
            { "key", _apiKey },
            { "image", base64Image }, // The base64 string
            { "name", fileName }
        };

        if (expirationInSeconds.HasValue)
        {
            parameters.Add("expiration", expirationInSeconds.Value.ToString());
        }

        using var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.PostAsync(UploadEndpoint, content);

        return await ProcessResponse(response);
    }

    /// <summary>
    /// Helper to deserialize the JSON response.
    /// </summary>
    private async Task<ImgBBResponse> ProcessResponse(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (!response.IsSuccessStatusCode)
        {
            // Try to parse the ImgBB error response
            var errorResponse = JsonSerializer.Deserialize<ImgBBResponse>(json, options);
            if (errorResponse?.Error != null)
            {
                throw new HttpRequestException($"ImgBB API Error: {errorResponse.Error.Message} (Code: {errorResponse.Error.Code})");
            }

            // Fallback for general HTTP errors
            response.EnsureSuccessStatusCode();
        }

        return JsonSerializer.Deserialize<ImgBBResponse>(json, options);
    }
}