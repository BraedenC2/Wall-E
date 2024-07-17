using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

public class OfflineLanguageModel
{
    private readonly HttpClient _httpClient;
    private const string API_URL = "http://localhost:8000/generate";

    public OfflineLanguageModel()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromMinutes(2);
    }

    public async Task<string> GenerateResponseAsync(string input)
    {
        try
        {
            Console.WriteLine($"Preparing to send request for input: {input}");
            var request = new HttpRequestMessage(HttpMethod.Post, API_URL);
            var jsonContent = JsonSerializer.Serialize(new { prompt = input });
            Console.WriteLine($"JSON content being sent: {jsonContent}");
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            Console.WriteLine($"Sending request to {API_URL}");
            var response = await _httpClient.SendAsync(request);

            Console.WriteLine($"Response received. Status code: {response.StatusCode}");
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw response body: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request failed. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }

            Console.WriteLine("Attempting to deserialize response");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = JsonSerializer.Deserialize<GenerationResult>(responseBody, options);

            if (result?.Response != null)
            {
                Console.WriteLine($"Deserialized response: {result.Response}");
                return result.Response;
            }
            else
            {
                Console.WriteLine("Deserialized response is null or empty");
                return "No valid response received";
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
            Console.WriteLine($"Raw response causing the error: {ex.Message}");
            return "Error processing server response";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error: {ex.Message}");
            return $"Error communicating with server: {ex.Message}";
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request timed out: {ex.Message}");
            return "Request to server timed out";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in GenerateResponseAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return $"Unexpected error: {ex.Message}";
        }
    }

    private class GenerationResult
    {
        public string Response { get; set; }
    }
}