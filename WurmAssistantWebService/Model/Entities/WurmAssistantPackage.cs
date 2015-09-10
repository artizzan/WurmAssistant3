using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Web;
using AldursLab.WurmAssistantWebService.Model.Entities.Base;
using AldursLab.WurmAssistantWebService.Validation;

namespace AldursLab.WurmAssistantWebService.Model.Entities
{
    public class WurmAssistantPackage : EntityBase
    {
        public WurmAssistantPackage()
        {
            WurmAssistantPackageId = Guid.NewGuid();
        }

        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid WurmAssistantPackageId { get; set; }

        [Required, MaxLength(250)]
        public string BuildCode { get; set; }

        [Required, MaxLength(250)]
        public string BuildNumber { get; set; }

        [Required]
        public virtual File File { get; set; }

        [ForeignKey("File")]
        public Guid FileId { get; set; }
    }
}