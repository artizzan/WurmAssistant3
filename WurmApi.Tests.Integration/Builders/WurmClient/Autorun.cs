using System.IO;
using AldursLab.WurmApi.Tests.Integration.Builders.WurmClient;

namespace AldursLab.WurmApi.Tests.Builders.WurmClient
{
    class Autorun
    {
        FileInfo AutorunTxt { get; set; }

        public Autorun(FileInfo autorunTxt)
        {
            AutorunTxt = autorunTxt;
            File.WriteAllText(AutorunTxt.FullName, Defaults.autorun);
        }
    }
}