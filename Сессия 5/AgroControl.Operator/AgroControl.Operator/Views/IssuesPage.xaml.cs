using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AgroControl.API.Models;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class IssuesPage : UserControl
    {
        private readonly ApiService _api;
        private List<EventLog> _allIssues = new();

        public IssuesPage(ApiService api)
        {
            InitializeComponent();
            _api = api;
            Loaded += async (s, e) => await LoadIssues();
        }

        private async System.Threading.Tasks.Task LoadIssues()
        {
            _allIssues = await _api.GetAllIssuesAsync() ?? new List<EventLog>();
            dgIssues.ItemsSource = _allIssues;
        }

        private async void Refresh_Click(object sender, System.Windows.RoutedEventArgs e) => await LoadIssues();

        private void Search_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var search = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search))
            {
                dgIssues.ItemsSource = _allIssues;
                return;
            }
            var filtered = _allIssues.Where(i => (i.ПартияПроизводстваID?.ToString().Contains(search) ?? false) ||
                                                  (i.Описание?.ToLower().Contains(search) == true) ||
                                                  (i.Создал?.ПолноеИмя?.ToLower().Contains(search) == true)).ToList();
            dgIssues.ItemsSource = filtered;
        }

        private void ClearSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtSearch.Clear();
            dgIssues.ItemsSource = _allIssues;
        }
    }
}