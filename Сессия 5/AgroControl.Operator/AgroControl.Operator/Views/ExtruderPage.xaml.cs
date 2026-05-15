using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class ExtruderPage : UserControl
    {
        private readonly ApiService _api;
        private DispatcherTimer? _timer;

        public ExtruderPage(ApiService api)
        {
            InitializeComponent();
            _api = api;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _timer.Tick += async (s, args) => await LoadData();
            _timer.Start();
            _ = LoadData();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => _timer?.Stop();

        private async Task LoadData()
        {
            var data = await _api.GetExtruderLiveDataAsync();
            if (data == null) return;
            lblTemp1.Text = $"{data.ТемператураЗоны1} °C";
            lblTemp2.Text = $"{data.ТемператураЗоны2} °C";
            lblPressure.Text = $"{data.Давление} атм";
            lblSpeed.Text = $"{data.СкоростьШнека} об/мин";
            lblPower.Text = $"{data.ТекущаяМощность} кВт";
            lblTime.Text = $"Обновлено: {data.ВремяРаботы}";
        }
    }
}