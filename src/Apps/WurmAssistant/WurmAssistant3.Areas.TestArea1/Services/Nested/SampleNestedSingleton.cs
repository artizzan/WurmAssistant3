﻿using AldursLab.WurmAssistant3.Areas.TestArea1.Contracts.Nested;

namespace AldursLab.WurmAssistant3.Areas.TestArea1.Services.Nested
{
    [KernelBind(BindingHint.Singleton)]
    public class SampleNestedSingleton : ISampleNestedSingleton
    {
    }
}
