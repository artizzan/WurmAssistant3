namespace AldursLab.WurmAssistant3.Areas.Core
{
    [KernelBind(BindingHint.FactoryProxy)]
    public interface ISendBugReportViewFactory
    {
        SendBugReportForm CreateSendBugReportView();
    }
}
