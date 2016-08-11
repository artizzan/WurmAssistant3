using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.Persistence;
using AldursLab.WurmAssistant3.Areas.Core;
using AldursLab.WurmAssistant3.Areas.Triggers.TriggersManager;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Areas.Triggers.ImportExport
{
    [KernelBind(BindingHint.Transient)]
    public class Exporter
    {
        readonly ISerializer serializer;
        readonly IFileDialogs fileDialogs;

        public Exporter([NotNull] ISerializer serializer, [NotNull] IFileDialogs fileDialogs)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            if (fileDialogs == null) throw new ArgumentNullException(nameof(fileDialogs));
            this.serializer = serializer;
            this.fileDialogs = fileDialogs;
        }

        public void Export(IEnumerable<ITrigger> triggers)
        {
            var dto = new TriggersDto()
            {
                TriggerEntities = triggers.Select(trigger => trigger.GetTriggerEntityCopy(serializer)).ToList()
            };
            var serialized = serializer.Serialize(dto);
            fileDialogs.SaveTextFile(serialized);
        }
    }
}
