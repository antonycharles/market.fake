using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Market.Domain.Exceptions;

namespace Market.Infrastructure.Repositories.Externals
{
    public class BaseClientRepository
    {
        protected readonly HttpClient _httpClient;
        protected BaseClientRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        protected async Task PutAsync(string url, object data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode == false)
                    await GenerateErrorException(response);

                return;
            }
            catch (ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }


        protected async Task PostAsync(string url, object data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode == false)
                    await GenerateErrorException(response);
                    
                return;
            }
            catch (ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }

        protected async Task<T> PostAsync<T>(string url, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode == false)
                    await GenerateErrorException(response);

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, GetJsonSerializerOptions());
            }
            catch (ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }
        
        protected async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content,GetJsonSerializerOptions());
            }
            catch(ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }

        protected async Task<T> PutAsync<T>(string url, object data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if(response.IsSuccessStatusCode == false)
                    await GenerateErrorException(response);

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent,GetJsonSerializerOptions());
            }
            catch(ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }

        protected async Task DeleteAsync(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);
            
                if(response.IsSuccessStatusCode == false)
                    await GenerateErrorException(response);
            }
            catch(ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }

        protected async Task<T> PostFormDataAsync<T>(string url, Dictionary<string, string> formData)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                foreach (var kvp in formData)
                {
                    content.Add(new StringContent(kvp.Value), kvp.Key);
                }

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    await GenerateErrorException(response);


                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, GetJsonSerializerOptions());
            }
            catch (ExternalApiException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ExternalApiException($"Error in request: {ex.Message}", ex);
            }
        }

        private static async Task GenerateErrorException(HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errors = JsonSerializer.Deserialize<JsonObject>(errorContent, GetJsonSerializerOptions());

            List<string> errorList = new List<string>();

            if (errors != null)
            {
                foreach (var error in errors.Where(w => w.Value != null))
                {
                    
                    if(error.Key == "detail")
                    {
                        var errorDetail = error.Value.ToString();
                        if (errorDetail != null)
                        {
                            errorList.Add(errorDetail);
                        }
                    }
                    else if (error.Key == "message")
                    {
                        var errorMessage = error.Value.ToString();
                        if (errorMessage != null)
                        {
                            errorList.Add(errorMessage);
                        }
                    }
                    else if (error.Key == "error")
                    {
                        var errorObject = error.Value.AsObject();
                        foreach (var item in errorObject)
                        {
                            errorList.Add(item.Value.ToString());
                        }
                    }
                    else if (error.Key == "errors")
                    {
                        if (error.Value is JsonObject errorObject)
                        {
                            foreach (var item in errorObject)
                            {
                                if (item.Value is JsonArray array)
                                {
                                    foreach (var arrayItem in array)
                                    {
                                        errorList.Add(arrayItem.ToString());
                                    }
                                }
                                else
                                {
                                    errorList.Add(item.Value.ToString());
                                }
                            }
                        }
                        else
                        {
                            var errorArray = error.Value.AsArray();
                            foreach (var item in errorArray)
                            {
                                errorList.Add(item.ToString());
                            }
                        }
                    }
                }
            }

            throw new ExternalApiException($"Error in request: {response.StatusCode}", errorList.ToArray());
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}