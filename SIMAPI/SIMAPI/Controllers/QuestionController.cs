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
            var setting = new ChatGPTHelper();

            var userString = await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "chatgptresponse.json"));
            var response = JsonConvert.DeserializeObject<ChatGPTResponse>(userString);

            var questionCollectionString = response.Choices.Select(s => s.Message.Content).FirstOrDefault();

            if (questionCollectionString.StartsWith("```json") && questionCollectionString.EndsWith("```"))
            {
                // Remove the ```json at the beginning and the closing ``` at the end
                questionCollectionString = questionCollectionString.Substring(7); // Remove "```json"
                questionCollectionString = questionCollectionString.Substring(0, questionCollectionString.LastIndexOf("```")); // Remove closing "```"
            }

            if (questionCollectionString != null)
            {
                var questionCollectionObject = JsonConvert.DeserializeObject<List<QuestionCollection>>(questionCollectionString);

                //var questions = questionCollectionObject.Select(q => new QuestionCollection
            
                return questionCollectionObject;
            }


            return new List<QuestionCollection>();
        }
    }
}
