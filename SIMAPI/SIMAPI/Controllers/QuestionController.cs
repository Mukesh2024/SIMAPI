using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SIMAPI.Helper;
using SIMAPI.Model;

namespace SIMAPI.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public async Task<GenerateQuestionResponse> GenerateQuestion(GenerateQuestionRequest model)
        {
            var generateQuestionResponse = new GenerateQuestionResponse();

            if (ModelState.IsValid)
            {
                var schema = new Schema();
                schema.Request = new List<Request>();

                var chatGPTHelper = new ChatGPTHelper();

                var chatGPTResponse = await chatGPTHelper.GenerateQuestion(model, _apiSettings.ApiKey, _apiSettings.Url, _apiSettings.Model);

                var data = JsonConvert.DeserializeObject<Schema>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "schema.json")));

                if (data.Request == null)
                {
                    data.Request = new List<Request>();
                }

                data.Request.Add(new Request
                {
                    Guid = Guid.NewGuid(),
                    UserRequest = model,
                    ChatGPTResponse = chatGPTResponse,
                    UserAnswer = null,
                    Status = "Pending"
                });

                var deserlize = JsonConvert.SerializeObject(data);

                await System.IO.File.WriteAllTextAsync(Path.Combine(_jsonFilePath, "schema.json"), deserlize);

                var questionCollection = chatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();

                //var data = JsonConvert.DeserializeObject<Schema>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "schema.json")));

                //generateQuestionResponse.Guid = data.Request.FirstOrDefault().Guid;

                //var questionCollection = data.Request.FirstOrDefault().ChatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();

                if (questionCollection.StartsWith("```json") && questionCollection.EndsWith("```"))
                {
                    // Remove the ```json at the beginning and the closing ``` at the end
                    questionCollection = questionCollection.Substring(7); // Remove "```json"
                    questionCollection = questionCollection.Substring(0, questionCollection.LastIndexOf("```")); // Remove closing "```"
                }

                if (questionCollection != null)
                {
                    var questionCollections = JsonConvert.DeserializeObject<List<QuestionCollection>>(questionCollection);

                    generateQuestionResponse.QuestionCollections = questionCollections;
                    questionCollections.ForEach(f =>
                    {
                        f.Answer = null;
                        f.Explanation = null;
                    });

                    return generateQuestionResponse;
                }

            }

            return generateQuestionResponse;
        }


        [HttpPost(Name = "SaveUserAnswer")]
        public async Task SaveUserAnswer(UserResponse model)
        {
            if (ModelState.IsValid)
            {
                var data = JsonConvert.DeserializeObject<Schema>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "schema.json")));

                if (data.Request != null)
                {
                    var request = data.Request.Where(w => w.Guid == model.Guid).FirstOrDefault();

                    var questionAnswer = request.ChatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();


                    if (questionAnswer.StartsWith("```json") && questionAnswer.EndsWith("```"))
                    {
                        // Remove the ```json at the beginning and the closing ``` at the end
                        questionAnswer = questionAnswer.Substring(7); // Remove "```json"
                        questionAnswer = questionAnswer.Substring(0, questionAnswer.LastIndexOf("```")); // Remove closing "```"
                    }

                    var questionCollections = JsonConvert.DeserializeObject<List<QuestionCollection>>(questionAnswer);

                    var totalCorrect = 0;
                    var totalInCorrect = 0;
                    var totalNotAttempt = 0;

                    model.Answers.ForEach(f =>
                    {
                        var question = questionCollections.FirstOrDefault(w => w.QuestionText.Trim() == f.Question);

                        if (question != null && (f.Answer != null && f.Answer != ""))
                        {
                            if (question.Answer == f.Answer)
                            {
                                f.IsCorrect = true;
                                totalCorrect++;
                            }
                            else
                            {
                                f.IsCorrect = false;
                                totalInCorrect++;
                            }

                            f.Explanation = question.Explanation;
                        }
                        else
                        {
                            f.IsCorrect = false;
                            totalNotAttempt++;
                        }
                    });

                    request.UserAnswer = model.Answers;
                    request.TotalCorrect = totalCorrect;
                    request.TotalInCorrect= totalInCorrect;
                    request.TotalNotAttempt= totalNotAttempt;

                    var deserlize = JsonConvert.SerializeObject(data);

                    await System.IO.File.WriteAllTextAsync(Path.Combine(_jsonFilePath, "schema.json"), deserlize);
                }
            }
        }
    }
}
