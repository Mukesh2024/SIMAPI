using SIMAPI.Enum;

namespace SIMAPI.Model
{
    public class Question
    {
        public string ChallengeName { get; set; }
        public Topics Topics { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int NumberOfQuestion { get; set; }
        public int TotalMarksOfEachAnswer { get; set; }
        public int TotalMarksOfEachCorrectAnswer { get; set; }
        public int TotalMarksDeductforEachWrongAnswer { get; set; }
        public int TotalTimeInMin { get; set; }
        public bool AllowAIGuidence { get; set; }
    }

    public class Topics
    {
        public string Subject { get; set; }
        public string Topic { get; set; }
    }
}
