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
            new { Title = "Algebra Basics", Grade = "A+", Date = DateTime.Now.AddDays(-2).ToString("yyyy-mm-dd") },
            new { Title = "Newton's Laws", Grade = "B", Date = DateTime.Now.AddDays(-5).ToString("yyyy-mm-dd") },
            new { Title = "Organic Chemistry", Grade = "A", Date = DateTime.Now.AddDays(-10).ToString("yyyy-mm-dd") }
        };

        return Ok(new { recentChallenges });
    }
    //GET /api/myChallenges/all-challenges
    [HttpGet("all-challenges")]
    public IActionResult GetAllChallenges()
    {
        var challenges = new List<object>
        {
            new
            {
                Title = "Algebra Test", Grade = "B+", Date = "2025-03-01", Subject = "Math"
            },
              new
            {
                Title = "Geometry Test", Grade = "A+", Date = "2025-03-01",  Subject = "Math"
            },
                new
            {
                Title = "Physics Test", Grade = "C", Date = "2025-03-01",  Subject = "Physics"
            },
                  new
            {
                Title = "Biology Test", Grade = "A+", Date = "2025-03-01",  Subject = "Biology"
            },
                    new
            {
                Title = "English Test", Grade = "B", Date = "2025-03-01",  Subject = "English"
            },
                      new
            {
                Title = "Chemistry Test", Grade = "C+", Date = "2025-03-01",  Subject = "Chemistry"
            },
                        new
            {
                Title = "General Test", Grade = "A", Date = "2025-03-01",  Subject = ""
            },
                          new
            {
                Title = "Algebra Test", Grade = "B+", Date = "2025-03-01",  Subject = "Math"
            }
        };
        return Ok(challenges);
    }

    // GET /api/dashboard/expertise-topics
    [HttpGet("expertise-topics")]
    public IActionResult GetExpertiseTopics()
    {
        var expertiseTopics = new List<object>
        {
            new { Title = "Quadratic Equations", Subject = "Maths", Question = "Solve x² - 4x + 4 = 0", Accuracy = 85, TotalQuestions = 12 },
            new { Title = "Momentum", Subject = "Physics", Question = "Calculate momentum of a 2kg object moving at 3m/s", Accuracy = 90, TotalQuestions = 9},
             new { Title = "Periodic Table & Chemical Reactions", Subject = "Bio", Question = "Calculate momentum of a 2kg object moving at 3m/s", Accuracy = 90, TotalQuestions = 99}
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
 
 