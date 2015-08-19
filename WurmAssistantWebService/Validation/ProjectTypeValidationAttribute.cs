using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AldursLab.WurmAssistantWebService.Model.Entities;
using AldursLab.WurmAssistantWebService.Validation.Base;

namespace AldursLab.WurmAssistantWebService.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProjectTypeValidationAttribute : ValidationAttribute
    {
        readonly EnumValidator<ProjectType> validator;

        public ProjectTypeValidationAttribute(params ProjectType[] allowedTypes)
        {
            validator = new EnumValidator<ProjectType>(allowedTypes);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return validator.IsValid(value, validationContext);
        }
    }
}