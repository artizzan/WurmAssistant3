using AldursLab.WurmAssistant3.Areas.Core.Transients;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    [NinjectFactory]
    public interface ISendBugReportViewFactory
    {
        SendBugReportForm CreateSendBugReportView();
    }
}
