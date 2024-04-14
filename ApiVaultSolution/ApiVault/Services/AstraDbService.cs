using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata;
using Newtonsoft.Json.Linq;
using ApiVault.DataModels;
using System.Text.Json;
using System.Text;

public class AstraDbService
{
    private readonly HttpClient _httpClient;
    private readonly string _astraDbId;
    private readonly string _astraDbRegion;
    private readonly string _astraDbKeyspace;
    private readonly string _astraDbApplicationToken;

    public AstraDbService(HttpClient httpClient, string astraDbId, string astraDbRegion, string astraDbKeyspace, string astraDbApplicationToken)
    {
        _httpClient = httpClient;
        _astraDbId = astraDbId;
        _astraDbRegion = astraDbRegion;
        _astraDbKeyspace = astraDbKeyspace;
        _astraDbApplicationToken = astraDbApplicationToken;
    }

    private void SetHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-cassandra-token", _astraDbApplicationToken);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    private void SetPostHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-cassandra-token", _astraDbApplicationToken);
        _httpClient.DefaultRequestHeaders.Add("content-type", "application/json");
    }

    public async Task<bool> DeleteApiKeyAsync(string tableName, string primaryKeyValue)
    {
        try
        {
            SetHeaders();

            string endpoint = $"https://{_astraDbId}-{_astraDbRegion}.apps.astra.datastax.com/api/rest/v2/keyspaces/{_astraDbKeyspace}/{tableName}/{primaryKeyValue}";

            // Send DELETE request
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

            // Check if the delete was successful
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("API key deleted successfully.");
                return true;
            }
            else
            {
                Debug.WriteLine($"Error deleting API key: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }

    public async Task<HttpContent?> GetUserCredentials(string tableName, string primaryKeyValue)
    {
        try
        {
            SetHeaders();

            // route and headers
            string endpoint = $"https://{_astraDbId}-{_astraDbRegion}.apps.astra.datastax.com/api/rest/v2/keyspaces/{_astraDbKeyspace}/{tableName}/{primaryKeyValue}";

            // send get request
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }

            else
            {
                Debug.WriteLine($"Error fetching credentials: {response.StatusCode}");
                return null;
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }

    public async Task<string?> GetApiKeys(string tableName, string username)
    {
        try
        {
            // set headers
            SetHeaders();

            // link and query
            string endpoint = $"https://{_astraDbId}-{_astraDbRegion}.apps.astra.datastax.com/api/rest/v2/keyspaces/{_astraDbKeyspace}/{tableName}/rows";
            string query = $"?where={{\"user_id\":{{\"$eq\":\"{username}\"}}}}";
            string fullRoute = endpoint + query;

            // Make request
            HttpResponseMessage response = await _httpClient.GetAsync(fullRoute);

            if ( response.IsSuccessStatusCode )
            {
                return await response.Content.ReadAsStringAsync();
            }

            else
            {
                Debug.WriteLine($"Failed to fetch keys: {response.StatusCode}");
                return null;
            }

        }

        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }

    }

    public async Task<bool> InsertUser(string tableName, string email, string username, string password, string phone)
    {
        // set headers
        SetHeaders();

        // endpoint
        string endpoint = $"https://{_astraDbId}-{_astraDbRegion}.apps.astra.datastax.com/api/rest/v2/keyspaces/{_astraDbKeyspace}/{tableName}";

        // Content
        ApiUser newUser = new ApiUser
        {
            username = username,
            email = email,
            password = password,
            phone = phone
        };

        string newUserJson = JsonSerializer.Serialize(newUser);
        Debug.WriteLine($"{newUserJson}");

        HttpContent requestContent = new StringContent(newUserJson, Encoding.UTF8, "application/json");

        try
        {
            // Make request
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, requestContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error Response: {errorContent}");
                return false;
            }
        }

        catch(Exception e)
        {
            Debug.WriteLine($"InsertUser catch: {e}");
            return false;
        }
    }
}
