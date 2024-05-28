using Microsoft.AspNetCore.Http;

namespace VehicleVault.Core.Settings.Attributes
{
    public class AllowedImagesSize : ValidationAttribute
    {
        private readonly int _allowedsize;
        public AllowedImagesSize(int _allowedsize)
        {
            this._allowedsize = _allowedsize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > _allowedsize)
                    {
                        return new ValidationResult($"Maximum allowed size is {_allowedsize} bytes");
                    }
                }
            }
            return ValidationResult.Success;

        }
    }
}
