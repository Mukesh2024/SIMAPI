using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

[Route("api/dashboard")]
[ApiController]
public class DashboardController : ControllerBase
{
    // GET /api/dashboard/recent-challenges
    [HttpGet("recent-challenges")]
    public IActionResult GetRecentChallenges()
    {
        var recentChallenges = new List<object>
        {
            new { Title = "Algebra Basics", Grade = "A+", Date = DateTime.Now.AddDays(-2) },
            new { Title = "Newton's Laws", Grade = "B", Date = DateTime.Now.AddDays(-5) },
            new { Title = "Organic Chemistry", Grade = "A", Date = DateTime.Now.AddDays(-10) }
        };

        return Ok(new { recentChallenges });
    }

    // GET /api/dashboard/expertise-topics
    [HttpGet("expertise-topics")]
    public IActionResult GetExpertiseTopics()
    {
        var expertiseTopics = new List<object>
        {
            new { Title = "Quadratic Equations", Subject = "Mathematics", Question = "Solve x² - 4x + 4 = 0", Accuracy = 85, TotalQuestions = 12 },
            new { Title = "Momentum", Subject = "Physics", Question = "Calculate momentum of a 2kg object moving at 3m/s", Accuracy = 90, TotalQuestions = 9}
        };

        return Ok(new { expertiseTopics });
    }

    // GET /api/dashboard/get-grade-class/{grade}
    [HttpGet("get-grade-class/{grade}")]
    public IActionResult GetGradeClass(string grade)
    {
        var gradeClasses = new Dictionary<string, string>
        {
            { "A+", "grade-excellent" },
            { "A", "grade-good" },
            { "B", "grade-average" },
            { "C", "grade-below-average" },
            { "D", "grade-poor" }
        };

        return Ok(new { grade, gradeClass = gradeClasses.GetValueOrDefault(grade, "grade-unknown") });
    }

    // GET /api/dashboard/get-subject-class/{subject}
    [HttpGet("get-subject-class/{subject}")]
    public IActionResult GetSubjectClass(string subject)
    {
        var subjectClasses = new Dictionary<string, string>
        {
            { "Mathematics", "subject-math" },
            { "Physics", "subject-physics" },
            { "Chemistry", "subject-chemistry" },
            { "Biology", "subject-biology" }
        };

        return Ok(new { subject, subjectClass = subjectClasses.GetValueOrDefault(subject, "subject-unknown") });
    }
}
 
 