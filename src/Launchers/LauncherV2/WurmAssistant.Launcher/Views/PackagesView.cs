using System;
using System.Linq;
using System.Windows.Forms;
using AldursLab.WurmAssistant.Shared.Dtos;

namespace AldursLab.WurmAssistant.Launcher.Views
{
    public partial class PackagesView : Form
    {
        public PackagesView(Package[] packages)
        {
            InitializeComponent();

            foreach (
                var package in
                    packages.OrderBy(package => GetMajorOrder(package.BuildCode))
                            .ThenBy(package => package.BuildCode)
                            .ThenByDescending(package => package.BuildNumber))
            {
                listBox1.Items.Add(new ViewItem(package));
            }
        }

        int GetMajorOrder(string buildCode)
        {
            if (buildCode.StartsWith("stable", StringComparison.InvariantCultureIgnoreCase)) 
                return -1;
            if (buildCode.StartsWith("beta", StringComparison.InvariantCultureIgnoreCase))
                return 0;
            if (buildCode.StartsWith("dev", StringComparison.InvariantCultureIgnoreCase))
                return 1;
            return 2;
        }

        class ViewItem
        {
            readonly Package package;

            public ViewItem(Package package)
            {
                this.package = package;
            }

            public override string ToString()
            {
                if (package == null) return "null";
                return string.Format("{0} : {1}", package.BuildCode, package.BuildNumber);
            }
        }
    }
}
