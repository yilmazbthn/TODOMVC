using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoMVC;

public interface IRecaptchaValidator
{
    Task<bool> IsValidAsync(string token);
}

public class RecaptchaValidator : IRecaptchaValidator
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;

    public RecaptchaValidator(IConfiguration configuration, HttpClient httpClient)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
    }

    public async Task<bool> IsValidAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            
            return false;
        }

        try
        {
            var secret = configuration.GetValue<string>("RecaptchaSecret");
            var parameters = new Dictionary<string, string?>
            {
                { "secret", secret },
                { "response", token }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var responseContent = await response.Content.ReadAsStringAsync();

            var recaptchaResponse = JsonSerializer.Deserialize<RecaptchaResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (recaptchaResponse == null || !recaptchaResponse.Success)
                return false;
            
            if (recaptchaResponse.Score.HasValue && recaptchaResponse.Score.Value < 0.5)
                return false;

            return true;
        }
        catch
        {
            
            return false;
        }
    }
}

public class RecaptchaResponse
{
    public bool Success { get; set; }
    public float? Score { get; set; }
    public string Action { get; set; }
    [JsonPropertyName("error-codes")]
    public List<string> ErrorCodes { get; set; }
}
