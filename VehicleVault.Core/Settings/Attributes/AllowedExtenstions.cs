using Microsoft.AspNetCore.Http;

namespace VehicleVault.Core.Settings.Attributes
{
    public class AllowedExtenstions : ValidationAttribute
    {
        private readonly string _allowedExtension;
        public AllowedExtenstions(string _allowedExtension)
        {
            this._allowedExtension = _allowedExtension;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var files = value as List<IFormFile>;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var isAllowed = _allowedExtension.Split(',').Contains(extension, StringComparer.OrdinalIgnoreCase);
                    if (!isAllowed)
                    {
                        return new ValidationResult($"Only {_allowedExtension} are allowed");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
