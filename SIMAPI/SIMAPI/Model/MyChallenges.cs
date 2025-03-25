namespace SIMAPI.Model
{
    public class MyChallenges
    {
        public string Name { get; set; }
        public List<SubjectAndTopics>  SubjectAndTopics { get; set; }
        public DateTime CompltedOn { get; set; }
        public string Grade { get; set; }
        public Guid Guid { get; set; }
    }
}
