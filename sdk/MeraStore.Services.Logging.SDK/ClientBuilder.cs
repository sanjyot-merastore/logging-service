using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace MeraStore.Services.Logging.SDK;

public class ClientBuilder
{
  private readonly IServiceCollection _services;
  private readonly IHttpClientBuilder _httpClientBuilder;
  private string _baseUrl = string.Empty;
  private IAsyncPolicy<HttpResponseMessage> _resiliencePolicy;

  public ClientBuilder()
  {
    _services = new ServiceCollection();
    _httpClientBuilder = _services.AddHttpClient<LoggingApiClient>();
    _resiliencePolicy = GetDefaultResiliencePolicy();
  }

  public ClientBuilder WithUrl(string baseUrl)
  {
    _baseUrl = baseUrl.TrimEnd('/'); // Remove trailing slash
    _httpClientBuilder.ConfigureHttpClient(client =>
    {
      client.BaseAddress = new Uri(_baseUrl);
      client.Timeout = TimeSpan.FromSeconds(30); // Set timeout
    });
    return this;
  }

  public ClientBuilder WithResiliencePolicy(IAsyncPolicy<HttpResponseMessage> resiliencePolicy)
  {
    _resiliencePolicy = resiliencePolicy;
    return this;
  }

  public ClientBuilder UseDefaultResiliencePolicy()
  {
    _resiliencePolicy = GetDefaultResiliencePolicy();
    return this;
  }

  public LoggingApiClient Build()
  {
    _httpClientBuilder.AddPolicyHandler(_resiliencePolicy);
    var provider = _services.BuildServiceProvider();
    return provider.GetRequiredService<LoggingApiClient>();
  }

  private static IAsyncPolicy<HttpResponseMessage> GetDefaultResiliencePolicy()
  {
    return Policy.WrapAsync([
      HttpPolicyExtensions
      .HandleTransientHttpError() // Handle transient HTTP errors
      .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Optionally handle specific status codes
      .WaitAndRetryAsync(3, // Retry 3 times
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // Exponential backoff
      ),
      Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)), // Timeout for requests
    ]);
  }
}
