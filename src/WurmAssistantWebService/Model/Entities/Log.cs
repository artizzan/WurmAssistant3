using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using AldursLab.WurmAssistantWebService.Model.Entities.Base;

namespace AldursLab.WurmAssistantWebService.Model.Entities
{
    public class Log : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }
        [MaxLength(2048)]
        public string Source { get; set; }
        public string Content { get; set; }
    }
}