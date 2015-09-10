using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AldursLab.WurmAssistantWebService.Model.Entities;

namespace AldursLab.WurmAssistantWebService.Model.Services
{
    public class Logs
    {
        readonly ApplicationDbContext applicationDbContext = new ApplicationDbContext();

        public void Add(string source, string content)
        {
            applicationDbContext.Logs.Add(new Log()
            {
                Source = source,
                Content = content
            });
            applicationDbContext.SaveChanges();
        }
    }
}