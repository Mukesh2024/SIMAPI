using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using SIMAPI.Model;
using System.Text;

namespace SIMAPI.Helper
{
    public class ChatGPTHelper
    {
        public async Task<ChatGPTResponse> GenerateQuestion(GenerateQuestionRequest model, string apiKey, string url, string chatGPTModel)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                string jsonRequest = GenerateRquest(model, chatGPTModel);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonConvert.DeserializeObject<ChatGPTResponse>(responseString);
                    //return jsonResponse.choices[0].text.ToString().Trim();
                    return jsonResponse;
                }
                //else
                //{
                //    //return $"Error: {responseString}";
                //}

                return null;

            }
        }


        private string GenerateRquest(GenerateQuestionRequest model, string chatGPTModel)
        {
            var systemContent = "You are an expert educational AI that generates structured multiple-choice quiz questions for high school and competitive exam students.";
            string topics = string.Empty;
            
            if (model.SubjectAndTopics.Count > 0)
            {
                foreach (var topic in model.SubjectAndTopics)
                {
                    if (topics != string.Empty)
                    {
                        topics = topics + " and ";
                        topics = topics +  string.Join(", ", topic.Topic) + " in " + topic.Subject;
                    }
                    else
                    {
                        topics = string.Join(", ", topic.Topic) + " in " + topic.Subject;
                    }
                }
            }


            var userContent = $"Generate {model.NumberOfQuestion} {model.DifficultyLevel.GetDisplayName()} multiple-choice question on {topics}.\n" +
                      "Each question should include:\n" +
                      "1. Question statement\n" +
                      "2. Four options (A to D)\n" +
                      "3. Correct answer (as A/B/C/D)\n" +
                      "4. Explanation (1-2 lines)\n\n" +
                      "Respond in JSON format like this:\n" +
                      "[\n" +
                      "  {\n" +
                      "    \\\"question\\\": \\\"What is the sum of the roots of the equation x² - 5x + 6 = 0?\\\",\n" +
                      "    \\\"options\\\": {\n" +
                      "      \\\"A\\\": \\\"5\\\",\n" +
                      "      \\\"B\\\": \\\"-5\\\",\n" +
                      "      \\\"C\\\": \\\"6\\\",\n" +
                      "      \\\"D\\\": \\\"-6\\\"\n" +
                      "    },\n" +
                      "    \\\"answer\\\": \\\"A\\\",\n" +
                      "    \\\"explanation\\\": \\\"Sum of roots = -b/a = -(-5)/1 = 5\\\"\n" +
                      "  }\n" +
                      "]";

            var requesBody = new ChatGPTRequest
            {
                model = chatGPTModel,
                messages = new Message[]
                {
                    new Message
                    {
                        role = "system",
                        content = systemContent
                    },
                    new Message
                    {
                        role = "user",
                        content = userContent
                    },
                }
            };

            var stringData = JsonConvert.SerializeObject(requesBody);
            return stringData;
        }

        //    public async Task<object> SendRequest()
        //    {
        //        var prompt = "Hello, how are you?"; // Input prompt to ask the model

        //        var response = await GetChatGptResponse(prompt);

        //        Console.WriteLine("Response from gpt-4o-mini: ");
        //        Console.WriteLine(response);
        //        return response;
        //    }

        //    private static async Task<string> GetChatGptResponse(string prompt)
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            // Set up the request headers
        //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        //            var requestBody = new
        //            {
        //                model = "gpt-4o-mini",  // Use "gpt-4" or another chat model like "gpt-3.5-turbo"
        //                messages = new[]
        // {
        //    new
        //    {
        //        role = "system",
        //        content = "You are an expert educational AI that generates structured multiple-choice quiz questions for high school and competitive exam students."
        //    },
        //    new
        //    {
        //        role = "user",
        //        content = "Generate 1 easy-level multiple-choice question on 'Quadratic Equations' in Mathematics.\n" +
        //                  "Each question should include:\n" +
        //                  "1. Question statement\n" +
        //                  "2. Four options (A to D)\n" +
        //                  "3. Correct answer (as A/B/C/D)\n" +
        //                  "4. Explanation (1-2 lines)\n\n" +
        //                  "Respond in JSON format like this:\n" +
        //                  "[\n" +
        //                  "  {\n" +
        //                  "    \\\"question\\\": \\\"What is the sum of the roots of the equation x² - 5x + 6 = 0?\\\",\n" +
        //                  "    \\\"options\\\": {\n" +
        //                  "      \\\"A\\\": \\\"5\\\",\n" +
        //                  "      \\\"B\\\": \\\"-5\\\",\n" +
        //                  "      \\\"C\\\": \\\"6\\\",\n" +
        //                  "      \\\"D\\\": \\\"-6\\\"\n" +
        //                  "    },\n" +
        //                  "    \\\"answer\\\": \\\"A\\\",\n" +
        //                  "    \\\"explanation\\\": \\\"Sum of roots = -b/a = -(-5)/1 = 5\\\"\n" +
        //                  "  }\n" +
        //                  "]"
        //    }
        //}
        //            };



        //            // Convert the request body to JSON
        //            string jsonRequest = JsonConvert.SerializeObject(requestBody);
        //            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        //            // Send the request
        //            var response = await client.PostAsync(apiUrl, content);

        //            // Read the response and parse it
        //            var responseString = await response.Content.ReadAsStringAsync();

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Parse the JSON response
        //                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
        //                //return jsonResponse.choices[0].text.ToString().Trim();
        //                return jsonResponse;
        //            }
        //            else
        //            {
        //                return $"Error: {responseString}";
        //            }
        //        }
        //    }
    }
}
