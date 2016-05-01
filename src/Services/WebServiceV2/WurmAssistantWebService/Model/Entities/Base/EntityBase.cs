using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AldursLab.WurmAssistantWebService.Model.Entities.Base
{
    public abstract class EntityBase
    {
        protected EntityBase()
        {
            var now = DateTimeOffset.Now;
            Created = now;
            Modified = now;
        }

        [Required]
        public DateTimeOffset Created { get; set; }

        [Required]
        public DateTimeOffset Modified { get; set; }
    }
}