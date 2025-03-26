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
        public async Task<Guid> GenerateQuestion(GenerateQuestionRequest model)
        {
            Guid guid = Guid.Empty;

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

                guid = Guid.NewGuid();

                data.Request.Add(new Request
                {
                    Guid = guid,
                    UserRequest = model,
                    ChatGPTResponse = chatGPTResponse,
                    UserAnswer = null,
                    Status = "Pending"
                });

                var deserlize = JsonConvert.SerializeObject(data);

                await System.IO.File.WriteAllTextAsync(Path.Combine(_jsonFilePath, "schema.json"), deserlize);
            }

            return guid;
        }

        [HttpGet(Name = "GetQuestion")]
        public async Task<GenerateQuestionResponse> GetQuestion([FromQuery] Guid model)
        {
            var generateQuestionResponse = new GenerateQuestionResponse();

            if (ModelState.IsValid)
            {

                var data = JsonConvert.DeserializeObject<Schema>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "schema.json")));

                if (data.Request != null)
                {
                    var requestObject = data.Request.Where(w => w.Guid == model).FirstOrDefault();
                    var questionCollection = requestObject.ChatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();

                    generateQuestionResponse.QuestionDetails = requestObject.UserRequest;
                    generateQuestionResponse.Guid = requestObject.Guid;

                    //var questionCollection = data.Request.Where(w => w.Guid == model).FirstOrDefault().ChatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();


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
                        });

                        generateQuestionResponse.QuestionCollections = questionCollections;

                        return generateQuestionResponse;
                    }
                }
            }

            return generateQuestionResponse;
        }

        [HttpPost(Name = "SaveUserAnswer")]
        public async Task<MyChallenges> SaveUserAnswer(UserResponse model)
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
                        var question = questionCollections.FirstOrDefault(w => w.QuestionText.Trim() == f.QuestionText);

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
                        }
                        else
                        {
                            f.IsCorrect = false;
                            totalNotAttempt++;
                        }

                        f.Hint = question.Hint;
                        f.Options = question.Options;
                        f.CorrectAnswer = question.Answer;
                    });

                    request.UserAnswer = model.Answers;
                    request.TotalCorrect = totalCorrect;
                    request.TotalInCorrect = totalInCorrect;
                    request.TotalNotAttempt = totalNotAttempt;

                    var percentile = (totalCorrect / (double)questionCollections.Count) * 100;

                    if (percentile >= 90)
                    {
                        request.Grade = "A+";
                    }
                    else if (percentile >= 80)
                    {
                        request.Grade = "A";
                    }
                    else if (percentile >= 70)
                    {
                        request.Grade = "B+";
                    }
                    else if (percentile >= 60)
                    {
                        request.Grade = "B";
                    }
                    else
                    {
                        request.Grade = "C";
                    }

                    request.Status = "Completed";
                    request.CompletedOn = DateTime.Now;

                    var chatGPTHelper = new ChatGPTHelper();

                    var chatGPResponse = await chatGPTHelper.GenerateAIRecommendationOnResult(model, _apiSettings.ApiKey, _apiSettings.Url, _apiSettings.Model);

                    var aiRecommendationOnResult = new AIRecommendationOnResult()
                    {
                        ChatGPTResponse = chatGPResponse,
                        Guid = model.Guid,
                    };

                    request.AIRecommendation = chatGPResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();

                    var aIRecommendationOnResultdData = JsonConvert.DeserializeObject<List<AIRecommendationOnResult>>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "AIRecommendationOnResult.json")));

                    if (aIRecommendationOnResultdData == null)
                    {
                        aIRecommendationOnResultdData = new List<AIRecommendationOnResult>();
                    }

                    aIRecommendationOnResultdData.Add(aiRecommendationOnResult);

                    await System.IO.File.WriteAllTextAsync(Path.Combine(_jsonFilePath, "AIRecommendationOnResult.json"), JsonConvert.SerializeObject(aIRecommendationOnResultdData));

                    var deserlize = JsonConvert.SerializeObject(data);

                    await System.IO.File.WriteAllTextAsync(Path.Combine(_jsonFilePath, "schema.json"), deserlize);

                    var myChallenges = new MyChallenges();
                    myChallenges.Guid = request.Guid;
                    myChallenges.Name = request.UserRequest.ChallengeName;
                    myChallenges.CompltedOn = request.CompletedOn;
                    myChallenges.Grade = request.Grade;
                    myChallenges.SubjectAndTopics = request.UserRequest.SubjectAndTopics;
                    myChallenges.TotalCorrect = request.TotalCorrect;
                    myChallenges.TotalInCorrect = request.TotalInCorrect;
                    myChallenges.TotalNotAttempt = request.TotalNotAttempt;
                    myChallenges.AIRecommendation = request.AIRecommendation;

                    return myChallenges;
                }
            }
            return new MyChallenges();
        }

        [HttpGet(Name = "MyChallanges")]
        public async Task<List<MyChallenges>> MyChallanges()
        {
            var data = JsonConvert.DeserializeObject<Schema>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "schema.json")));

            var challanged = new List<MyChallenges>();

            data.Request.ForEach(f =>
            {
                var myChallenges = new MyChallenges();
                myChallenges.Guid = f.Guid;
                myChallenges.Name = f.UserRequest.ChallengeName;
                myChallenges.CompltedOn = f.CompletedOn;
                myChallenges.Grade = f.Grade;
                myChallenges.SubjectAndTopics = f.UserRequest.SubjectAndTopics;
                myChallenges.TotalCorrect = f.TotalCorrect;
                myChallenges.TotalInCorrect = f.TotalInCorrect;
                myChallenges.TotalNotAttempt = f.TotalNotAttempt;
                myChallenges.AIRecommendation = f.AIRecommendation;
                challanged.Add(myChallenges);
            });

            return challanged;

        }

        [HttpGet(Name = "SubjectAndTopics")]
        public async Task<List<SubjectAndTopics>> SubjectAndTopics()
        {
            return JsonConvert.DeserializeObject<List<SubjectAndTopics>>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "Subject.json")));
        }

        [HttpGet(Name = "GetQuestionWithAnswer")]
        public async Task<QuestionWIthAnswer> GetQuestionWithAnswer(Guid model)
        {
            var questionWIthAnswer = new QuestionWIthAnswer();

            if (model != Guid.Empty)
            {
                var data = JsonConvert.DeserializeObject<Schema>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "schema.json")));

                var userQuestionAndAnswer = data.Request.FirstOrDefault(f => f.Guid == model);
                questionWIthAnswer.UserAnswer = userQuestionAndAnswer.UserAnswer;
                questionWIthAnswer.Details= userQuestionAndAnswer.UserRequest;
                questionWIthAnswer.TotalCorrect = userQuestionAndAnswer.TotalCorrect;
                questionWIthAnswer.TotalInCorrect = userQuestionAndAnswer.TotalInCorrect;
                questionWIthAnswer.TotalNotAttempt = userQuestionAndAnswer.TotalNotAttempt;

                return questionWIthAnswer;
            }
            else
            {
                return questionWIthAnswer;
            }
        }

        [HttpPost(Name = "RecommendationOnQuestion")]
        public async Task<string> RecommendationOnQuestion(RecommendationOnQuestion model)
        {
            var recommendationText = string.Empty;

            if (ModelState.IsValid)
            {
                var data = JsonConvert.DeserializeObject<List<AIRecommendation>>(await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "AIRecommendation.json")));

                if (data != null)
                {
                    var recommendation = data.FirstOrDefault(f => f.RecommendationOnQuestion.Guid == model.Guid
                    && f.RecommendationOnQuestion.QuestionDetail.QuestionText == model.QuestionDetail.QuestionText);

                    if (recommendation != null)
                    {
                        recommendationText = recommendation.ChatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();
                    }
                }

                if (recommendationText == "")
                {
                    data = new List<AIRecommendation>();

                    var chatGPTHelper = new ChatGPTHelper();

                    var chatGPTResponse = await chatGPTHelper.GenerateAIRecommendation(model, _apiSettings.ApiKey, _apiSettings.Url, _apiSettings.Model);

                    data.Add(new AIRecommendation
                    {
                        ChatGPTResponse = chatGPTResponse,
                        RecommendationOnQuestion = model
                    });

                    var deserlize = JsonConvert.SerializeObject(data);

                    await System.IO.File.WriteAllTextAsync(Path.Combine(_jsonFilePath, "AIRecommendation.json"), deserlize);

                    recommendationText = chatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();

                    recommendationText = chatGPTResponse.Choices.Select(s => s.Message.Content).FirstOrDefault();
                }
            }

            if (recommendationText.StartsWith("```html") && recommendationText.EndsWith("```"))
            {
                recommendationText = recommendationText.Substring(7);
                recommendationText = recommendationText.Substring(0, recommendationText.LastIndexOf("```"));
            }
            return recommendationText;
        }
    }
}
