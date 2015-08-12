using System;

namespace AldursLab.WurmAssistant3.Attributes
{
    /// <summary>
    /// Declares, that decorated class is an Wurm Assistant Module
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class WurmAssistantModuleAttribute : Attribute
    {
        public string ModuleId { get; private set; }

        public WurmAssistantModuleAttribute(string moduleId)
        {
            ModuleId = moduleId;
        }
    }
}
