using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly LearningContext _dbContext;

        public PaymentService(HttpClient httpClient, LearningContext dbContext)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.sandbox.paypal.com"); // PayPal Sandbox base URL
            _dbContext = dbContext;
        }

        public async Task<string> CreatePayPalPaymentAsync(string amount)
        {
            // Replace these values with your PayPal API credentials
            string clientId = "ATo5OxwAnxj2Mv0fFMqn3EAOoLz8zmQ_PXM7E_ZZNRLaNR4wofa8w7WigCObgskRoOGsUnuCQMloguFk";
            string clientSecret = "EBEHJ76KH7befsvrdnQqAjW6URjhMoNGkxsuN6FqjOf5E5h1lS6_apMaSTvG6ltonKHln8-05NfrgYmS";

            // Generate access token
            string accessToken = await GetAccessTokenAsync(clientId, clientSecret);

            // Construct request body
            var requestBody = new Dictionary<string, object>
            {
                { "intent", "sale" },
                { "payer", new Dictionary<string, string> { { "payment_method", "paypal" } } },
                { "transactions", new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            { "amount", new Dictionary<string, string> { { "total", amount }, { "currency", "USD" } } }
                        }
                    }
                },
                { "redirect_urls", new Dictionary<string, string>
                    {
                        { "return_url", "http://localhost:5039/Payment/Success" },
                        { "cancel_url", "https://yourdomain.com/cancel" }
                    }
                }
            };

            // Serialize request body
            var requestBodyJson = JsonConvert.SerializeObject(requestBody);

            // Send HTTP POST request to PayPal API to create payment
            var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsync("/v1/payments/payment", content);

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Parse response to get approval URL
            var responseJson = await response.Content.ReadAsStringAsync();
            var approvalUrl = JsonConvert.DeserializeObject<dynamic>(responseJson)["links"][1]["href"].ToString();

            return approvalUrl;
        }

        private async Task<string> GetAccessTokenAsync(string clientId, string clientSecret)
        {
            // Construct basic authentication header
            string authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            // Send HTTP POST request to PayPal API to get access token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };
            var tokenRequestContent = new FormUrlEncodedContent(tokenRequest);
            var tokenResponse = await _httpClient.PostAsync("/v1/oauth2/token", tokenRequestContent);
            tokenResponse.EnsureSuccessStatusCode();

            // Parse response to get access token
            var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<dynamic>(tokenResponseJson)["access_token"].ToString();

            return accessToken;
        }

        public async Task StorePaymentDetailsAsync(string paymentId,  string payerId)
        {
            var paymentDetails = new Enrollment
            {
                
                PaymentId = paymentId,
                TransactionId = payerId
            };

            _dbContext.Enrollments.Add(paymentDetails);
            await _dbContext.SaveChangesAsync();
        }
    }

}
