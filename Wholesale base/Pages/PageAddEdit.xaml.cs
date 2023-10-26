using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wholesale_base.Classes;

namespace Wholesale_base.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddEdit.xaml
    /// </summary>
    public partial class PageAddEdit : Page
    {
        private Supplies _currentSupplies = new Supplies();
        public PageAddEdit(Supplies selectedSupplies)
        {
            InitializeComponent();
            if (selectedSupplies != null)

                _currentSupplies = selectedSupplies;

            DataContext = _currentSupplies;

            Cmbid_provaider.ItemsSource = Wholesale_baseEntities.GetContext().provider.ToList();
            Cmbid_provaider.SelectedValuePath = "id_provaider";
            Cmbid_provaider.DisplayMemberPath = "Name";

            Cmbid_product.ItemsSource = Wholesale_baseEntities.GetContext().Product.Distinct().ToList();
            Cmbid_product.SelectedValuePath = "id_product";
            Cmbid_product.DisplayMemberPath = "Product_name";

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentSupplies.delivery_price))
                errors.AppendLine("Укажите цену доставки");
            if (string.IsNullOrWhiteSpace(_currentSupplies.delivery_time))
                errors.AppendLine("Укажите время доставки");
            if (string.IsNullOrWhiteSpace(_currentSupplies.quantity))
                errors.AppendLine("Укажите количество");
            if (string.IsNullOrWhiteSpace(_currentSupplies.Account_number))
                errors.AppendLine("Укажите номер счета");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentSupplies.id_Supplies == 0)
                Wholesale_baseEntities.GetContext().Supplies.Add(_currentSupplies);

            try
            {
                Wholesale_baseEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Classes.ClassFrame.frmObj.Navigate(new Pages.Page1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }
        }
    }
}

