using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required, MaxLength(20)]
        public string Extension { get; set; }

        [NotMapped]
        public string CombinedName { get { return Name + "." + Extension; } }
        
        [Required]
        public Byte[] Contents { get; set; }

        
    }
}