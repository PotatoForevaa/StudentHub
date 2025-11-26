using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxSize;
        public FileSizeAttribute(long maxSize) 
        {
            _maxSize = maxSize;
            ErrorMessage = $"Размер файла превышает допустимый {maxSize / 1024 / 1024} МБ";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile file || file.Length == 0) return ValidationResult.Success;
            var size = file.Length;

            return size <= _maxSize
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}
