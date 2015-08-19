using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AldursLab.WurmAssistantWebService.Model.Entities;
using AldursLab.WurmAssistantWebService.Validation.Base;

namespace AldursLab.WurmAssistantWebService.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReleaseTypeValidationAttribute : ValidationAttribute
    {
        readonly EnumValidator<ReleaseType> validator;

        public ReleaseTypeValidationAttribute(params ReleaseType[] allowedTypes)
        {
            validator = new EnumValidator<ReleaseType>(allowedTypes);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return validator.IsValid(value, validationContext);
        }
    }
}