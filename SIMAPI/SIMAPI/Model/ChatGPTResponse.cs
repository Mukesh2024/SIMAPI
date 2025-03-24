using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SIMAPI.Model
{
    public class Option
    {
        public string OptionKey { get; set; }
        public string OptionValue { get; set; }
    }

    public class Questions
    {
        public string QuestionText { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public string Answer { get; set; }
        public string Explanation { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Messages Message { get; set; }
    }

    public class Messages
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class ChatGPTResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }
    
}
