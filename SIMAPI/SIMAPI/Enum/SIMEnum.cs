using System.ComponentModel.DataAnnotations;

namespace SIMAPI.Enum
{
    public enum DifficultyLevel
    {
        [Display(Name = "Easy-Level")]
        Easy,
        [Display(Name = "Medium-Level")]
        Medium,
        [Display(Name = "Hard-Level")]
        Hard,
        [Display(Name = "Advanced-Level")]
        Advanced
    }

}
