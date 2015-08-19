using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AldursLab.WurmAssistantWebService.Model.Entities;

namespace AldursLab.WurmAssistantWebService.Validation.Base
{
    public class EnumValidator<TEnum> where TEnum : struct
    {
        private readonly TEnum[] allowedTypes;

        public EnumValidator(IEnumerable<TEnum> allowedTypes)
        {
            this.allowedTypes = allowedTypes.ToArray();
        }

        public ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("value is null");
            }
            if (!(value is ProjectType))
            {
                return
                    new ValidationResult(string.Format("Invalid value type, expected {0}, actual: {1}",
                        typeof (TEnum).FullName,
                        value.GetType().FullName));
            }
            var castValue = (TEnum) value;
            if (!allowedTypes.Contains(castValue))
            {
                return
                    new ValidationResult(string.Format("Type {0} is not allowed, allowed types: {1}",
                        castValue,
                        string.Join(", ", allowedTypes)));
            }

            return ValidationResult.Success;
        }

    }
}