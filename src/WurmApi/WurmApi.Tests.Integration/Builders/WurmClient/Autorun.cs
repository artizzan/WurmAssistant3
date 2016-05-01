using System.IO;

namespace AldursLab.WurmApi.Tests.Integration.Builders.WurmClient
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