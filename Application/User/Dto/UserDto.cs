using Core.Attribute;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.User.Dto
{
    public class UserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]  
        [SafeInputValidation]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [SafeInputValidation]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "UserName")]
        [SafeInputValidation]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Password { get; set; }

        public Guid? RoleId { get; set; }

        [MobileNumber]
        public string PhoneNo { get; set; }

        [Required]
        [StringLength(10)]
        public string NationalCode { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "Format Date To Be YYYY/MM/DD")]
        [CustomValidation(typeof(DateValidator), "ValidateDate")]
        public string CreateDate { get; set; }
    }
}
