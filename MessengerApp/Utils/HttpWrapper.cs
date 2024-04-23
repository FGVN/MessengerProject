using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class HttpWrapper
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public HttpWrapper(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<TResponse> GetAsync<TResponse>(string url)
    {
        using (var response = await _httpClient.GetAsync(_baseUrl + url))
        {
            return await HandleResponse<TResponse>(response);
        }
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        using (var response = await _httpClient.PostAsJsonAsync(_baseUrl + url, data))
        {
            return await HandleResponse<TResponse>(response);
        }
    }

    public async Task<TResponse> GetAsync<TResponse>(string url, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        using (var response = await _httpClient.GetAsync(_baseUrl + url))
        {
            return await HandleResponse<TResponse>(response);
        }
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        using (var response = await _httpClient.PostAsJsonAsync(_baseUrl + url, data))
        {
            return await HandleResponse<TResponse>(response);
        }
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        using (var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"))
        {
            using (var response = await _httpClient.PutAsync(_baseUrl + url, content))
            {
                return await HandleResponse<TResponse>(response);
            }
        }
    }

    public async Task DeleteAsync(string url, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        using (var response = await _httpClient.DeleteAsync(_baseUrl + url))
        {
            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}. Response body: {responseBody}");
            }
        }
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        else
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"HTTP request failed with status code {response.StatusCode}. Response body: {responseBody}");
        }
    }
}
