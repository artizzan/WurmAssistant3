using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AldursLab.WurmAssistantWebService.Model.Entities.Base;

namespace AldursLab.WurmAssistantWebService.Model.Entities
{
    public class File : EntityBase
    {
        public File()
        {
            FileId = Guid.NewGuid();
        }

        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid FileId { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public string StorageFileName { get; set; }
    }
}