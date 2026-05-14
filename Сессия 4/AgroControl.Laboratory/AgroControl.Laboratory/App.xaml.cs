using System.Windows;
using OfficeOpenXml;

namespace AgroControl.Laboratory
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ExcelPackage.License.SetNonCommercialPersonal("AgroControl Laboratory");
            base.OnStartup(e);
        }
    }
}