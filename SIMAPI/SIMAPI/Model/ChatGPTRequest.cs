using System.Text.Json.Serialization;

namespace SIMAPI.Model
{
    public class ChatGPTRequest
    {
        public string model { get; set; }
        public Message[] messages { get; set; }

        //[JsonIgnore]
        //public string Result { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
