using System.Collections.Generic;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Modules;

namespace AldursLab.WurmAssistant3.Core.Areas.WurmApi.Contracts
{
    public interface IWurmClientValidator
    {
        bool SkipOnStart { get; set; }
        IReadOnlyList<WurmClientIssue> Validate();
        void ShowSummaryWindow(IReadOnlyList<WurmClientIssue> issues);
    }
}