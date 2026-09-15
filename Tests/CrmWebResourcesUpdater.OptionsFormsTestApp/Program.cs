using System;
using System.Windows.Forms;

namespace CrmWebResourcesUpdater.OptionsForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WebResourcesUpdaterForm());
        }
    }
}
