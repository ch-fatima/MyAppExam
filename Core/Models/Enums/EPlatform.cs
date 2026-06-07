using System.ComponentModel.DataAnnotations;

namespace Core.Models.Enums
{
    public enum EPlatform
    {
        [Display(Name = "اندروید")]
        Android = 0,

        [Display(Name = "آی او اس")]
        Ios = 1,

        [Display(Name = "وب")]
        Web = 2,
    }
}
