namespace SIMAPI.Model
{
    public class Schema
    {
        public List<Request> Request { get; set; }
    }

    public class Request
    {
        public Guid Guid { get; set; }
        public GenerateQuestionRequest UserRequest { get; set; }
        public ChatGPTResponse ChatGPTResponse { get; set; }
        public List<UserAnswer>? UserAnswer { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalInCorrect { get; set; }
        public int TotalNotAttempt { get; set; }
        public string  Grade { get; set; }
        public string  Status { get; set; }
        public DateTime  CompletedOn { get; set; }
        public string? AIRecommendation { get; set; }
    }

}
