using System.ComponentModel.DataAnnotations;

namespace Core.Models.Enums
{
    public enum UserSex
    {
        [Display(Name = "نامشخص")]
        Unknown = 0,

        [Display(Name = "مرد")]
        Male = 1,

        [Display(Name = "زن")]
        Female = 2,
    }
}
