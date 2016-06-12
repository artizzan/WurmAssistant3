using AldursLab.WurmAssistant3.Areas.Core.Services;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    [NinjectFactory]
    public interface ISendBugReportViewFactory
    {
        SendBugReportForm CreateSendBugReportView();
    }
}
