using Newtonsoft.Json;
using System.Text;

namespace SIMAPI.Helper
{
    public class ChatGPTHelper
    {
        // Replace this with your OpenAI API key
        private static string apiKey = "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";
        private static string apiUrl = "https://api.openai.com/v1/chat/completions\r\n";  // Use this URL for GPT-3.5 or earlier models

        public async Task<object> SendRequest()
        {
            var prompt = "Hello, how are you?"; // Input prompt to ask the model

            var response = await GetChatGptResponse(prompt);


            Console.WriteLine("Response from gpt-4o-mini: ");
            Console.WriteLine(response);
            return response;
        }

        private static async Task<string> GetChatGptResponse(string prompt)
        {
            using (var client = new HttpClient())
            {
                // Set up the request headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-4o-mini",  // Use "gpt-4" or another chat model like "gpt-3.5-turbo"
                    messages = new[]
                   {
                        new { role = "user", content = prompt }
                    }
                };


                // Convert the request body to JSON
                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Send the request
                var response = await client.PostAsync(apiUrl, content);

                // Read the response and parse it
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                    return jsonResponse.choices[0].text.ToString().Trim();
                }
                else
                {
                    return $"Error: {responseString}";
                }
            }
        }
    }
}
