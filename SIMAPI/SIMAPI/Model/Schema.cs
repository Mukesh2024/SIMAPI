namespace SIMAPI.Model
{
    public class Schema
    {
        public List<Request> Request { get; set; }
    }

    public class Request
    {
        public Guid Guid { get; set; }
        public Question UserRequest { get; set; }
        public ChatGPTResponse ChatGPTResponse { get; set; }
        public string UserAnswer { get; set; }
        public string  Status { get; set; }
    }

}
