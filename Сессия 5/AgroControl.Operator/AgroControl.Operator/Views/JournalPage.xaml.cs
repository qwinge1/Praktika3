using System.Linq;
using System.Windows.Controls;
using AgroControl.Operator.Services;

namespace AgroControl.Operator.Views
{
    public partial class JournalPage : UserControl
    {
        private readonly ApiService _api;
        private readonly int _batchId;

        public JournalPage(ApiService api, int batchId)
        {
            InitializeComponent();
            _api = api;
            _batchId = batchId;
            Loaded += async (s, e) => await LoadEvents();
        }

        private async System.Threading.Tasks.Task LoadEvents()
        {
            var events = await _api.GetBatchEventsAsync(_batchId);
            if (events != null)
                dgEvents.ItemsSource = events.OrderByDescending(e => e.ВремяСобытия);
        }
    }
}