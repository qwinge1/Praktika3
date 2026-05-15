using System.Windows;

namespace AgroControl.Operator
{
    public partial class App : Application
    {
        public static string? CurrentUser { get; set; }
        public static int CurrentUserId { get; set; } = 14; // test.user по умолчанию
    }
}