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
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
            dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.ToList();

            Cmbquantity.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.Select(x => x.quantity).Distinct().ToList();
            CmbAccount_number.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.Select(x => x.Account_number).Distinct().ToList();
        }

        private void Cmbquantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string quantity = Cmbquantity.SelectedValue.ToString();
            dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.Where(x => x.quantity == quantity).ToList();
        }

        private void CmbAccount_number_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Account_number = CmbAccount_number.SelectedValue.ToString();
            dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.Where(x => x.Account_number == Account_number).ToList();
        }

        private void BtnResetFiltr_Click(object sender, RoutedEventArgs e)
        {
            dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.ToList();
        }

        private void Txtdelivery_price_TextChanged(object sender, TextChangedEventArgs e)
        {
            string delivery_price = Txtdelivery_price.Text;
            dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.Where(x => x.delivery_price.Contains(delivery_price)).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Classes.ClassFrame.frmObj.Navigate(new Pages.PageAddEdit(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Wholesale_baseEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.ToList();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

            Classes.ClassFrame.frmObj.Navigate(new Pages.PageAddEdit((sender as Button).DataContext as Supplies));
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var Remove = dtgSupplies.SelectedItems.Cast<Supplies>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {Remove.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Wholesale_baseEntities.GetContext().Supplies.RemoveRange(Remove);
                    Wholesale_baseEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");

                    dtgSupplies.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void BtnLISTVIEW_Click(object sender, RoutedEventArgs e)
        {
            Classes.ClassFrame.frmObj.Navigate(new Pages.PageList());
        }
    }
}
