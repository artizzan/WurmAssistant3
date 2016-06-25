using AldursLab.WurmAssistant3.Areas.Core.Services;

namespace AldursLab.WurmAssistant3.Areas.Core.Contracts
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISendBugReportViewFactory
    {
        SendBugReportForm CreateSendBugReportView();
    }
}
