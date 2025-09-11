using Shared.Core.Enums;

namespace PaymentService.Application
{
    public class TaskServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _taskServiceUrl = "https://localhost:7298/api/Admin/users";
        public TaskServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // Example method to get tasks for a user
        public async Task UpdateUserRoleAsync(int userId, UserRole role)
        {

            var request = new HttpRequestMessage(HttpMethod.Put, $"{_taskServiceUrl}/{userId}/{role}");
            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { Role = role.ToString() }), System.Text.
                Encoding.UTF8,
                "application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
