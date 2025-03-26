namespace SIMAPI.Model
{
    public class QuestionWIthAnswer
    {
        public Guid Guid { get; set; }
        public GenerateQuestionRequest Details  { get; set; }
        public List<UserAnswer> UserAnswer { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalInCorrect { get; set; }
        public int TotalNotAttempt { get; set; }
    }
}
