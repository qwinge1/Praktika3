using System.Windows;
using OfficeOpenXml;

namespace AgroControl.Technologist
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Настройка лицензии EPPlus (добавьте это перед любым использованием библиотеки)
            ExcelPackage.License.SetNonCommercialPersonal("AgroControl User");

            base.OnStartup(e);
        }
    }
}