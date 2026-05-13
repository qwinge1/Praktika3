using System.Windows;
using System.Windows.Controls;

namespace AgroControl.Technologist.Views
{
    public partial class ProductEditDialog : Window
    {
        public string Code => codeBox.Text.Trim();
        public string Name => nameBox.Text.Trim();
        public string ProductType => typeBox.Text.Trim();
        public string Form => formBox.Text.Trim();
        public string Status => ((ComboBoxItem)statusCombo.SelectedItem)?.Content?.ToString() ?? "черновик";

        public ProductEditDialog()
        {
            InitializeComponent();
        }

        public ProductEditDialog(string code, string name, string productType, string form, string status) : this()
        {
            codeBox.Text = code;
            nameBox.Text = name;
            typeBox.Text = productType;
            formBox.Text = form;
            foreach (ComboBoxItem item in statusCombo.Items)
                if (item.Content.ToString() == status)
                { statusCombo.SelectedItem = item; break; }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Код и наименование обязательны!");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}