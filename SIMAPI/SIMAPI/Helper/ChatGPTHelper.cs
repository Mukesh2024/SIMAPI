﻿using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using SIMAPI.Model;
using System.Reflection;
using System.Text;

namespace SIMAPI.Helper
{
    public class ChatGPTHelper
    {
        public async Task<ChatGPTResponse> GenerateQuestion(GenerateQuestionRequest model, string apiKey, string url, string chatGPTModel)
        {
            string jsonRequest = GenerateQuestionRquest(model, chatGPTModel);
            return await SendRequestToChatGPT(jsonRequest, apiKey, url);
        }

        public async Task<ChatGPTResponse> GenerateAIRecommendation(RecommendationOnQuestion model, string apiKey, string url, string chatGPTModel)
        {
            string jsonRequest = GenerateAIRecommndationOnQuestionRequest(model, chatGPTModel);
            return await SendRequestToChatGPT(jsonRequest, apiKey, url);
        }

        public async Task<ChatGPTResponse> GenerateAIRecommendationOnResult(UserResponse model, string apiKey, string url, string chatGPTModel)
        {
            string jsonRequest = GenerateAIRecommndationOnResultRequest(model, chatGPTModel);
            return await SendRequestToChatGPT(jsonRequest, apiKey, url);
        }

        private async Task<ChatGPTResponse> SendRequestToChatGPT(string jsonRequest, string apiKey, string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonConvert.DeserializeObject<ChatGPTResponse>(responseString);
                    return jsonResponse;
                }
                else
                {
                    return new ChatGPTResponse();
                }
            }
        }

        private string GenerateQuestionRquest(GenerateQuestionRequest model, string chatGPTModel)
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
                        topics = topics + string.Join(", ", topic.Topic) + " in " + topic.Subject;
                    }
                    else
                    {
                        topics = string.Join(", ", topic.Topic) + " in " + topic.Subject;
                    }
                }
            }


            var userContent = $"Generate {model.NumberOfQuestion} {model.DifficultyLevel.GetDisplayName()} multiple-choice question on {topics} for grade {model.Grade}.\n" +
                      "Each question should include:\n" +
                      "1. Question statement\n" +
                      "2. Four options (A to D)\n" +
                      "3. Correct answer (as A/B/C/D)\n" +
                      (model.AllowAIGuidence ? "4. Hint (1 lines)\n\n" : string.Empty) +
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
                      (model.AllowAIGuidence ? "    \\\"hint\\\": \\\"Sum of roots = -b/a = -(-5)/1 = 5\\\"\n" : string.Empty) +
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

        private string GenerateAIRecommndationOnQuestionRequest(RecommendationOnQuestion model, string chatGPTModel)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append($"Below are the detail of student mock test, We need some recommndation as per answer given by student for grade {model.Grade} student.\r\n");
            sb.Append($"Below are the detail of student mock test results, Guide student how we can improve on result as per answer given by student for grade {model.Grade} student.\r\n");
            sb.Append("Question :");
            sb.Append(model.QuestionDetail.QuestionText + ".\r\n");
            sb.Append("Options :");
            sb.Append(JsonConvert.SerializeObject(model.QuestionDetail.Options) + "\r\n");
            sb.Append("Answer: ");
            sb.Append(model.QuestionDetail.Answer != ""? model.QuestionDetail.Answer:"Not attempt"  + "\r\n");
            sb.Append("The question options Pleaese compare the answer with given option and then tell it is writh or wrong.");
            sb.Append("If answer value 'Not attempt' then don't tell 'Your answer is right or wrong' just give the explaination.");
            sb.Append("If the answer is incorrect, provide guidance on what the student can do or avoid in the future.");
            sb.Append("If the answer is wrong, offer motivational feedback like, You're learning and improving—keep practicing! You’ll get it right next time. 😊👏");
            sb.Append("If the answer is correct, suggest ways the student can improve further.");
            sb.Append("If the answer is correct, offer positive reinforcement such as, You're doing amazing—keep up the hard work! 🎉👏");
            sb.Append("Keep the responses clear and simple so that they are easily displayed on an HTML page with appropriate emotions.");
            sb.Append("Tell student that answer is wrong or right");
            sb.Append("Explain the question and how it can be solve to student");
            sb.Append("How student can improve himself");
            sb.Append("Give response in html and don't use html,head and body tag, use only inline css");
            sb.Append("Keep it clean, professional, and engaging.");
            sb.Append("Pleaese compare the answer with option.");



            //sb.Append("In response only recommndation part should be but with well documentation as HTML format and use only div with inline css, don't use margin and use prefesstional theme with elegance color for title, subtitle and headings.");

            var requesBody = new ChatGPTRequest
            {
                model = chatGPTModel,
                messages = new Message[]
                {
                     new Message
                    {
                        role = "system",
                        content = "You are the analyst."
                    },
                    new Message
                    {
                        role = "user",
                        content = sb.ToString()
                    },
                }
            };

            var stringData = JsonConvert.SerializeObject(requesBody);
            return stringData;
        }

        private string GenerateAIRecommndationOnResultRequest(UserResponse model, string chatGPTModel)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Below are the detail of student {model.Grade} in json format.\r\n");
            sb.Append(JsonConvert.SerializeObject(model.Answers) + "\r\n");
            sb.Append("Suppose you are teacher and give recommendation like a teacher in 3-4 line with bullats\tand you don't need to explain like \"As a teacher\"\r\n");

            var requesBody = new ChatGPTRequest
            {
                model = chatGPTModel,
                messages = new Message[]
                {
                     new Message
                    {
                        role = "system",
                        content = "You are the Teacher."
                    },
                    new Message
                    {
                        role = "user",
                        content = sb.ToString()
                    },
                }
            };

            var stringData = JsonConvert.SerializeObject(requesBody);
            return stringData;
        }
    }
}
