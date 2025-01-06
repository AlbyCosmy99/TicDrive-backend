using System.ComponentModel.DataAnnotations;

namespace TicDrive.Attributes
{
    public class RequiredIfUserTypeAttribute : ValidationAttribute
    {
        private readonly int _userTypeRequired;

        public RequiredIfUserTypeAttribute(int userTypeRequired)
        {
            _userTypeRequired = userTypeRequired;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var userTypeProperty = validationContext.ObjectType.GetProperty("UserType");
            if (userTypeProperty == null)
            {
                return new ValidationResult("UserType property is missing.");
            }

            var userType = (int)userTypeProperty.GetValue(instance, null);
            if (userType == _userTypeRequired)
            {
                if (value == null ||
                    (value is string strValue && string.IsNullOrWhiteSpace(strValue)) ||
                    (value is decimal decimalValue && decimalValue == default))
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"{validationContext.DisplayName} is required when UserType is {_userTypeRequired}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
