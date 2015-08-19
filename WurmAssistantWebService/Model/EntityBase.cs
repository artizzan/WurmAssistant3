using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AldursLab.WurmAssistantWebService.Model
{
    public abstract class EntityBase
    {
        protected EntityBase()
        {
            Created = DateTimeOffset.Now;
        }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTimeOffset Created { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTimeOffset? Modified { get; set; }
    }
}