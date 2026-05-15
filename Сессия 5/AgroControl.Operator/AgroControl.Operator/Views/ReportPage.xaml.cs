using System.Windows;
using System.Windows.Controls;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class ReportPage : UserControl
    {
        private readonly ApiService _api;
        private readonly int _batchId;

        public ReportPage(ApiService api, int batchId)
        {
            InitializeComponent();
            _api = api;
            _batchId = batchId;
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageBox.Show("Введите сообщение о проблеме", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var success = await _api.ReportIssueAsync(_batchId, txtMessage.Text, App.CurrentUserId);
            if (success)
            {
                MessageBox.Show("Сообщение отправлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                txtMessage.Clear();
            }
            else
            {
                MessageBox.Show("Не удалось отправить сообщение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}