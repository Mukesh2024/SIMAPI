using SIMAPI.Enum;

namespace SIMAPI.Model
{
    public class GenerateQuestionRequest
    {
        public string ChallengeName { get; set; }
        public List<SubjectAndTopics> SubjectAndTopics { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int NumberOfQuestion { get; set; }
        public int TotalMarksOfEachAnswer { get; set; }
        public int TotalMarksOfEachCorrectAnswer { get; set; }
        public int TotalMarksDeductforEachWrongAnswer { get; set; }
        public int TotalTimeInMin { get; set; }
        public string Grade { get; set; }
        public bool AllowAIGuidence { get; set; }
    }

    public class SubjectAndTopics  
    {
        public string Subject { get; set; }
        public string[] Topic { get; set; }
    }
}
