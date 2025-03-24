using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SIMAPI.Helper;
using SIMAPI.Model;

namespace SIMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Database");

        private readonly ChatGPTSetting _apiSettings;

        public QuestionController(IOptions<ChatGPTSetting> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        [HttpPost(Name = "GenerateQuestion")]
        public async Task<List<QuestionCollection>> GenerateQuestion(Question model)
        {
            var chatGPTHelper = new ChatGPTHelper();

            var chatGPTResponse = await chatGPTHelper.GenerateQuestion(model, _apiSettings.ApiKey, _apiSettings.Url, _apiSettings.Model);

            var questionCollection = chatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();

            //var userString = await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "chatgptresponsemultiresp.json"));
            //var response = JsonConvert.DeserializeObject<ChatGPTResponse>(userString);

            //var questionCollection = response.Choices.Select(s => s.Message.Content).FirstOrDefault();

            if (questionCollection.StartsWith("```json") && questionCollection.EndsWith("```"))
            {
                // Remove the ```json at the beginning and the closing ``` at the end
                questionCollection = questionCollection.Substring(7); // Remove "```json"
                questionCollection = questionCollection.Substring(0, questionCollection.LastIndexOf("```")); // Remove closing "```"
            }

            if (questionCollection != null)
            {
                var questionCollections = JsonConvert.DeserializeObject<List<QuestionCollection>>(questionCollection);

                questionCollections.ForEach(f =>
                {
                    f.Answer = null;
                    f.Explanation = null;
                });
                return questionCollections;
            }

            return new List<QuestionCollection>();
        }
    }
}
