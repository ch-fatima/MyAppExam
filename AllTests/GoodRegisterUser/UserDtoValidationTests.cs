using Application.User.Dto;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace AllTests.GoodRegisterUser
{
    public class UserDtoValidationTests
    {
        [Fact]
        public void UserDto_ValidData_ShouldPassValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void UserDto_EmptyFirstName_ShouldFailValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.FirstName = "";

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("FirstName"));
        }

        [Fact]
        public void UserDto_InvalidEmail_ShouldFailValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Email = "invalid-email";

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Invalid Email format"));
        }

        [Fact]
        public void UserDto_InvalidCreateDateFormat_ShouldFailValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.CreateDate = "24/12/25";

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("فرمت تاریخ باید YYYY/MM/DD باشد"));
        }

        [Fact]
        public void UserDto_InvalidCreateDateValue_ShouldFailValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.CreateDate = "2024/13/01";

            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("تاریخ وارد شده معتبر نیست"));
        }

        private UserDto CreateValidUserDto()
        {
            return new UserDto
            {
                NationalCode="2523654785",
                FirstName = "fateme",
                LastName = "chourli",
                Email = "chourli@gmail.com",
                UserName = "chourli",
                Password = "ValidPass123!",
                PhoneNo = "09999999999",
                RoleId = Guid.NewGuid(),
                CreateDate = "2024/12/25"
            };
        }
    }
}
