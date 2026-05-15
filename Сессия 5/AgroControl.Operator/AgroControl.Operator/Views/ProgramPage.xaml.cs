using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AgroControl.Operator.Services;
using Newtonsoft.Json.Linq;

namespace AgroControl.Operator.Views
{
    public partial class ProgramPage : UserControl
    {
        private readonly ApiService _api;
        private readonly int _batchId;
        private readonly MainWindow _parent;
        private JObject? _programData;
        private JObject? _currentStep;

        public ProgramPage(ApiService api, int batchId, MainWindow parent)
        {
            InitializeComponent();
            _api = api;
            _batchId = batchId;
            _parent = parent;
            Loaded += async (s, e) => await LoadProgram();
        }

        private async System.Threading.Tasks.Task LoadProgram()
        {
            try
            {
                var data = await _api.GetBatchProgramAsync(_batchId);
                if (data == null) return;

                var json = System.Text.Json.JsonSerializer.Serialize(data);
                _programData = JObject.Parse(json);

                var batch = _programData["batch"];
                var program = _programData["Program"] as JArray;
                if (program == null) return;

                lstSteps.ItemsSource = program;

                string productName = "—";
                try
                {
                    var order = batch?["Заказ"];
                    if (order != null && order["Продукт"] != null)
                        productName = order["Продукт"]["Наименование"]?.ToString() ?? "—";
                }
                catch { }

                lblBatchNumber.Text = batch?["НомерПартии"]?.ToString() ?? "—";
                lblProductLine.Text = $"{productName} / Линия L-01";

                var currentStepObj = program.FirstOrDefault(s => s["СтатусВыполнения"]?.ToString() == "выполняется");
                string currentStepName = currentStepObj?["НаименованиеШага"]?.ToString() ?? "—";
                lblStatusStep.Text = $"{batch?["Статус"]?.ToString() ?? "—"} / {currentStepName}";
                lblStartTime.Text = batch?["ВремяСтарта"]?.ToString() ?? "—";

                bool hasCritical = program.Any(s => s["Отклонение"]?.Type == JTokenType.Boolean && (bool?)s["Отклонение"] == true);
                lblDeviation.Visibility = hasCritical ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки программы партии: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Step_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentStep = lstSteps.SelectedItem as JObject;
            if (_currentStep == null) return;

            lblStepName.Text = $"{_currentStep["НомерШага"]}. {_currentStep["НаименованиеШага"]}";
            lblPlanTemp.Text = $"Плановая температура: {_currentStep["ПланТемпература"]} °C";
            lblPlanDuration.Text = $"Плановая длительность: {_currentStep["ПланДлительностьМинут"]} мин";
            lblPlanPressure.Text = $"Плановое давление: {_currentStep["ПланДавление"]} атм";
            lblInstruction.Text = $"Инструкция: {_currentStep["Инструкция"]}";

            txtActualTemp.Text = _currentStep["ФактТемпература"]?.ToString();
            txtActualDuration.Text = _currentStep["ФактДлительностьМинут"]?.ToString();
            txtActualPressure.Text = _currentStep["ФактДавление"]?.ToString();
            txtComment.Text = string.Empty;

            btnStart.IsEnabled = _currentStep["СтатусВыполнения"]?.ToString() == "не начат";
            btnComplete.IsEnabled = _currentStep["СтатусВыполнения"]?.ToString() == "выполняется";
        }

        private async void SaveActuals_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep == null) return;
            var executionIdToken = _currentStep["ВыполнениеID"];
            int? executionId = executionIdToken?.ToObject<int?>();
            if (executionId == null)
            {
                MessageBox.Show("Не удалось определить ID выполнения шага", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal? temp = decimal.TryParse(txtActualTemp.Text, out var t) ? t : null;
            int? dur = int.TryParse(txtActualDuration.Text, out var d) ? d : null;
            decimal? press = decimal.TryParse(txtActualPressure.Text, out var p) ? p : null;

            var success = await _api.UpdateActualsAsync(executionId.Value, temp, dur, press, txtComment.Text);
            if (success)
            {
                MessageBox.Show("Данные сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadProgram();
                Step_SelectionChanged(null, null);
            }
        }

        private async void StartStep_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep == null) return;
            var executionIdToken = _currentStep["ВыполнениеID"];
            int? executionId = executionIdToken?.ToObject<int?>();
            if (executionId == null)
            {
                MessageBox.Show("Не удалось определить ID выполнения шага", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = await _api.StartStepAsync(executionId.Value);
            if (success)
            {
                await LoadProgram();
                Step_SelectionChanged(null, null);
            }
        }

        private async void CompleteStep_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep == null) return;
            var executionIdToken = _currentStep["ВыполнениеID"];
            int? executionId = executionIdToken?.ToObject<int?>();
            if (executionId == null)
            {
                MessageBox.Show("Не удалось определить ID выполнения шага", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = await _api.CompleteStepAsync(executionId.Value);
            if (success)
            {
                await LoadProgram();
                Step_SelectionChanged(null, null);
            }
        }

        private async void RefreshProgram_Click(object sender, RoutedEventArgs e) => await LoadProgram();
        private async void DeleteBatch_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить партию {lblBatchNumber.Text}? Все данные выполнения шагов будут удалены.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            var success = await _api.DeleteBatchAsync(_batchId);
            if (success)
            {
                MessageBox.Show("Партия удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _parent.LoadActiveBatchesPage(); // возврат к списку активных партий
            }
            else
            {
                MessageBox.Show("Не удалось удалить партию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ReleaseProduct_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все обязательные шаги завершены
            var steps = lstSteps.ItemsSource as IEnumerable<dynamic>;
            if (steps != null && steps.Any(s => s.Обязательный == true && s.СтатусВыполнения != "завершен"))
            {
                MessageBox.Show("Не все обязательные шаги завершены! Невозможно выпустить продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем введённое количество
            if (!decimal.TryParse(txtProductQuantity.Text, out var quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество продукции (кг)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Завершаем партию через API
            var success = await _api.CompleteBatchAsync(_batchId, quantity);
            if (success)
            {
                MessageBox.Show($"Партия {lblBatchNumber.Text} завершена. Выпущено {quantity} кг продукции.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // Возвращаемся к списку активных партий
                _parent.LoadActiveBatchesPage();
            }
            else
            {
                MessageBox.Show("Не удалось завершить партию. Попробуйте позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}