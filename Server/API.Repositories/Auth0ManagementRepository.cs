using API.Domain;
using API.Interfaces.IRepositories;
using API.Repositories.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API.Repositories {
    public class Auth0ManagementRepository : IAuth0ManagementRepository {
        private static readonly string USERS_ENDPOINT = "api/v2/users";
        
        private readonly Auth0Options _options;
        private readonly HttpClient _client;

        public Auth0ManagementRepository(IOptionsSnapshot<Auth0Options>  options) {
            _options = options.Value;
            _client = new HttpClient {
                BaseAddress = new Uri($"https://{_options.Domain}/")
            };
        }

        public async Task<AccessToken> GetAccessToken() {
            _client.DefaultRequestHeaders.Clear();

            var body = new {
                grant_type = "client_credentials",
                client_id = _options.Management.ClientId,
                client_secret = _options.Management.ClientSecret,
                audience = $"https://{_options.Domain}/api/v2/"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage resp = await _client.PostAsync("oauth/token", content);
            string json = await resp.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AccessToken>(json);
        }

        public async Task<List<User>> GetUsersAsync(string access_token, long index, long size, string search) {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");

            var fieldsRequired = Uri.EscapeDataString("username,user_id,picture,name,nickname");

            string query = $"per_page={size}&page={index}&fields={fieldsRequired}";

            if(search != null) {
                search = Uri.EscapeDataString(search);
                query = $"{query}&q={search}";
            }

            string json = await _client.GetStringAsync($"{USERS_ENDPOINT}?{query}");

            return JsonConvert.DeserializeObject<List<User>>(json);
        }

        public Task<List<User>> GetUsersAsync(string access_token, long index, long size) {
            return GetUsersAsync(access_token, index, size, null);
        }

        public async Task<User> GetUserAsync(string userId, string access_token) {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");

            userId = Uri.EscapeDataString(userId);

            var fieldsRequired = Uri.EscapeDataString("username,user_id,picture,name,nickname");

            string json = await _client.GetStringAsync($"{USERS_ENDPOINT}/{userId}?fields={fieldsRequired}");

            return JsonConvert.DeserializeObject<User>(json);
        }

        public async Task<bool> UserExistsAsync(string userId, string access_token) {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");

            userId = Uri.EscapeDataString(userId);

            HttpResponseMessage resp = await _client.GetAsync($"{USERS_ENDPOINT}/{userId}?fields=user_id");

            return resp.IsSuccessStatusCode;
        }
    }
}
