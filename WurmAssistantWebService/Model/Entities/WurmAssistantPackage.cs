using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Web;
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

        [Required, RegularExpression(@"^\d+\.\d+\.\d+\.\d+$")]
        public string VersionString { get; set; }

        [NotMapped]
        public Version Version
        {
            get { return Version.Parse(VersionString); }
            set
            {
                Debug.Assert(value != null, "value != null");
                VersionString = value.ToString();
            }
        }

        [Required, ProjectTypeValidation(ProjectType.WurmAssistant3, ProjectType.WurmAssistantLite)]
        public ProjectType ProjectType { get; set; }

        [Required, ReleaseTypeValidation(ReleaseType.Beta, ReleaseType.Stable)]
        public ReleaseType ReleaseType { get; set; }

        [Required]
        public virtual File File { get; set; }

        [ForeignKey("File")]
        public Guid FileId { get; set; }
    }
}