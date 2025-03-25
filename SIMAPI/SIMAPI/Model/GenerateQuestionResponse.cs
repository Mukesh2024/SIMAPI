namespace SIMAPI.Model
{
    public class GenerateQuestionResponse
    {
        public Guid Guid { get; set; }
        public List<QuestionCollection> QuestionCollections { get; set; }
    }
}
