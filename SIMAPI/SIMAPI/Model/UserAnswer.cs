﻿namespace SIMAPI.Model
{
    public class UserResponse
    {
        public Guid Guid { get; set; }
        public List<UserAnswer> Answers { get; set; }
    }

    public class UserAnswer
    {
        public string Question { get; set; }
        public string? Answer { get; set; }
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }

    }

}
