using System.Collections.Generic;
using AldursLab.WurmAssistant3.Core.Areas.WurmApi.Modules;

namespace AldursLab.WurmAssistant3.Core.Areas.WurmApi.Contracts
{
    public interface IWurmClientValidator
    {
        /// <summary>
        /// Should validation check results be shown to user on app start.
        /// </summary>
        bool SkipOnStart { get; set; }

        /// <summary>
        /// Returns a list of issues found. List is empty if no issues found.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<WurmClientIssue> Validate();

        /// <summary>
        /// Shows a window presenting all issues.
        /// </summary>
        /// <param name="issues"></param>
        void ShowSummaryWindow(IReadOnlyList<WurmClientIssue> issues);
    }
}