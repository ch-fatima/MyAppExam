using Core.Attribute;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Dtos
{
    public class GetUserDto
    {
        [StringLength(50)]
        [SafeInputValidation]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [SafeInputValidation]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [MobileNumber]
        public string PhoneNo { get; set; }

        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "UserName")]
        [SafeInputValidation]
        public string UserName { get; set; } 
        public bool? IsActive { get; set; }  
    }
}
