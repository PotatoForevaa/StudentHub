using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FileExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public FileExtensionsAttribute(string extensions)
        {
            _extensions = extensions.Split(",").Select(x => x.Trim().ToLower()).ToArray();
            ErrorMessage = $"Недопустимое расширение файла";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile file || file.Length == 0) return ValidationResult.Success;
            var ext = Path.GetExtension(file.FileName)?.Split('.').Last().ToLower();

            return _extensions.Contains(ext)
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}
