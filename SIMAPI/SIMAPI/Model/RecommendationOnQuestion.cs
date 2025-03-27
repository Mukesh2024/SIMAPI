namespace SIMAPI.Model
{
    public class RecommendationOnQuestion
    {
        public Guid Guid { get; set; }
        public string Grade { get; set; }
        public UserAnswer QuestionDetail { get; set; }
    }
}
