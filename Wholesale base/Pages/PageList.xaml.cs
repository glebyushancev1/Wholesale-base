using System;
using System.Collections.Generic;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace Wholesale_base.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageList.xaml
    /// </summary>
    public partial class PageList : Page
    {
        public PageList()
        {
            InitializeComponent();
            //LViewWholesalebase.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.ToList();
            var currentSupplies = Wholesale_baseEntities.GetContext().Supplies.ToList();
            LViewWholesalebase.ItemsSource = currentSupplies;
            DataContext = LViewWholesalebase;
            Cmbquantity.Items.Add("Все пользователи");
            foreach (var item in Wholesale_baseEntities.GetContext().Supplies.
              Select(x => x.quantity).Distinct().ToList())
                Cmbquantity.Items.Add(item);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TxtSearch.Text;
            if (TxtSearch.Text != null)
            {
                LViewWholesalebase.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.
                    Where(x => x.Product.Product_name.Contains(search)
                    || x.provider.Name.Contains(search)
                    || x.delivery_price.Contains(search)
                    //|| x.payment_type.name.ToString().Contains(search)
                    || x.delivery_time.ToString().Contains(search)).ToList();

            }
        }

        private void BtnSaveToExcel_Click(object sender, RoutedEventArgs e)
        {
            //объект Excel
            var app = new Excel.Application();

            //книга 
            Excel.Workbook wb = app.Workbooks.Add();
            //лист
            Excel.Worksheet worksheet = app.Worksheets.Item[1];
            int indexRows = 1;
            //ячейка
            worksheet.Cells[1][indexRows] = "Номер";
            worksheet.Cells[2][indexRows] = "Название товара";
            worksheet.Cells[3][indexRows] = "Имя курьера";
            worksheet.Cells[4][indexRows] = "Цена доставки";
            worksheet.Cells[5][indexRows] = "Время доставки";
            worksheet.Cells[6][indexRows] = "Количество";

            //список пользователей из таблицы после фильтрации и поиска
            var printItems = LViewWholesalebase.Items;
            //цикл по данным из списка для печати
            foreach (Supplies item in printItems)
            {
                worksheet.Cells[1][indexRows + 1] = indexRows;
                worksheet.Cells[2][indexRows + 1] = item.Product.Product_name;
                worksheet.Cells[3][indexRows + 1] = item.provider.Name;
                worksheet.Cells[4][indexRows + 1] = item.delivery_price;
                worksheet.Cells[5][indexRows + 1] = item.delivery_time;
                worksheet.Cells[6][indexRows + 1].Value = item.quantity.ToString();

                indexRows++;
            }
            Excel.Range range = worksheet.Range[worksheet.Cells[2][indexRows + 1],
                    worksheet.Cells[5][indexRows + 1]];
            range.ColumnWidth = 30; //ширина столбцов
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;//выравнивание по левому краю

            //показать Excel
            app.Visible = true;
        }

        

        private void Cmbquantity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cmbquantity.SelectedValue.ToString() == "Все пользователи")
            {
                LViewWholesalebase.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.ToList();
            }
            else
            {
                LViewWholesalebase.ItemsSource = Wholesale_baseEntities.GetContext().Supplies.
                    Where(x => x.quantity == Cmbquantity.SelectedValue.ToString()).ToList();
            }
        }

        private void BtnSaveToExcelTemplate_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Open($"{Directory.GetCurrentDirectory()}\\Шаблон.xlsx");
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Cells[4, 2] = DateTime.Now.ToString();
            ws.Cells[4, 5] = 7;
            int indexRows = 6;
            //ячейка
            ws.Cells[1][indexRows] = "Номер";
            ws.Cells[2][indexRows] = "Название товара";
            ws.Cells[3][indexRows] = "Имя курьера";
            ws.Cells[4][indexRows] = "Цена доставки";
            ws.Cells[5][indexRows] = "Время доставки";
            ws.Cells[5][indexRows] = "Количество";

            //список пользователей из таблицы после фильтрации и поиска
            var printItems = LViewWholesalebase.Items;
            //цикл по данным из списка для печати
            foreach (Supplies item in printItems)
            {
                ws.Cells[1][indexRows + 1] = indexRows;
                ws.Cells[2][indexRows + 1] = item.Product.Product_name;
                ws.Cells[3][indexRows + 1] = item.provider.Name;
                ws.Cells[4][indexRows + 1] = item.delivery_price;
                ws.Cells[4][indexRows + 1] = item.delivery_time;
                ws.Cells[6][indexRows + 1].Value = item.quantity.ToString();

                indexRows++;
            }
            ws.Cells[indexRows + 2, 3] = "Подпись";
            ws.Cells[indexRows + 2, 5] = "Ющанцев Г.";
            excelApp.Visible = true;
        }

        private void BtnSaveToWord_Click(object sender, RoutedEventArgs e)
        {
            var allEmployees = Wholesale_baseEntities.GetContext().Supplies.ToList();

            var application = new Word.Application();

            Word.Document document = application.Documents.Add();

            Word.Paragraph empParagraph = document.Paragraphs.Add();
            Word.Range empRange = empParagraph.Range;
            empRange.Text = "Сотрудники";
            empRange.Font.Bold = 4;
            empRange.Font.Italic = 4;
            empRange.Font.Color = Word.WdColor.wdColorBlue;
            empRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table paymentsTable = document.Tables.Add(tableRange, allEmployees.Count() + 1, 5);
            paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;

            cellRange = paymentsTable.Cell(1, 1).Range;
            cellRange.Text = "Название товара";
            cellRange = paymentsTable.Cell(1, 2).Range;
            cellRange.Text = "Имя курьера";
            cellRange = paymentsTable.Cell(1, 3).Range;
            cellRange.Text = "Цена доставки";
            cellRange = paymentsTable.Cell(1, 4).Range;
            cellRange.Text = "Время доставки";
            cellRange = paymentsTable.Cell(1, 5).Range;
            cellRange.Text = "Количество";

            paymentsTable.Rows[1].Range.Bold = 1;
            paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            for (int i = 0; i < allEmployees.Count(); i++)
            {
                var currentEmployee = allEmployees[i];

                //cellRange = paymentsTable.Cell(i + 2, 1).Range;
                //Word.InlineShape imageShape = cellRange.InlineShapes.AddPicture(AppDomain.CurrentDomain.BaseDirectory
                //    + "..\\..\\" + currentEmployee.photo);
                //imageShape.Width = imageShape.Height = 40;
                //cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                cellRange = paymentsTable.Cell(i + 2, 1).Range;
                cellRange.Text = currentEmployee.Product.Product_name;

                cellRange = paymentsTable.Cell(i + 2, 2).Range;
                cellRange.Text = currentEmployee.provider.Name;

                cellRange = paymentsTable.Cell(i + 2, 3).Range;
                cellRange.Text = currentEmployee.delivery_price;

                cellRange = paymentsTable.Cell(i + 2, 4).Range;
                cellRange.Text = currentEmployee.delivery_time.ToString();

                cellRange = paymentsTable.Cell(i + 2, 5).Range;
                cellRange.Text = currentEmployee.quantity.ToString();
            }
            Supplies maxSalary = Wholesale_baseEntities.GetContext().Supplies
                .OrderByDescending(p => p.delivery_price).FirstOrDefault();
            if (maxSalary != null)
            {
                Word.Paragraph maxSalaryParagraph = document.Paragraphs.Add();
                Word.Range maxSalaryRange = maxSalaryParagraph.Range;
                maxSalaryRange.Text = $"Самая дорогая доставка - {maxSalary.delivery_price}";
                maxSalaryRange.Font.Color = Word.WdColor.wdColorDarkRed;
                maxSalaryRange.InsertParagraphAfter();
            }

            Supplies minSalary = Wholesale_baseEntities.GetContext().Supplies
                .OrderBy(p => p.delivery_price).FirstOrDefault();
            if (minSalary != null)
            {
                Word.Paragraph minSalaryParagraph = document.Paragraphs.Add();
                Word.Range minSalaryRange = minSalaryParagraph.Range;
                minSalaryRange.Text = $"Самая дешевая доставка - {minSalary.delivery_price}";
                minSalaryRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                minSalaryRange.InsertParagraphAfter();
            }

            application.Visible = true;

            document.SaveAs2(@"C:\Users\bpvla\Desktop\270923 глеб\Wholesale base\Wholesale base\bin\Debug\Test.docx");
        }

        private void BtnSaveToPDF_Click(object sender, RoutedEventArgs e)
        {
            var allEmployees = Wholesale_baseEntities.GetContext().Supplies.ToList();

            var application = new Word.Application();

            Word.Document document = application.Documents.Add();

            Word.Paragraph empParagraph = document.Paragraphs.Add();
            Word.Range empRange = empParagraph.Range;
            empRange.Text = "Сотрудники";
            empRange.Font.Bold = 4;
            empRange.Font.Italic = 4;
            empRange.Font.Color = Word.WdColor.wdColorBlue;
            empRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table paymentsTable = document.Tables.Add(tableRange, allEmployees.Count() + 1, 5);
            paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;

            cellRange = paymentsTable.Cell(1, 1).Range;
            cellRange.Text = "Название товара";
            cellRange = paymentsTable.Cell(1, 2).Range;
            cellRange.Text = "Имя курьера";
            cellRange = paymentsTable.Cell(1, 3).Range;
            cellRange.Text = "Цена доставки";
            cellRange = paymentsTable.Cell(1, 4).Range;
            cellRange.Text = "Время доставки";
            cellRange = paymentsTable.Cell(1, 5).Range;
            cellRange.Text = "Количество";

            paymentsTable.Rows[1].Range.Bold = 1;
            paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            for (int i = 0; i < allEmployees.Count(); i++)
            {
                var currentEmployee = allEmployees[i];
                cellRange = paymentsTable.Cell(i + 2, 1).Range;
                cellRange.Text = currentEmployee.Product.Product_name;

                cellRange = paymentsTable.Cell(i + 2, 2).Range;
                cellRange.Text = currentEmployee.provider.Name;

                cellRange = paymentsTable.Cell(i + 2, 3).Range;
                cellRange.Text = currentEmployee.delivery_price;

                cellRange = paymentsTable.Cell(i + 2, 4).Range;
                cellRange.Text = currentEmployee.delivery_time;

                cellRange = paymentsTable.Cell(i + 2, 5).Range;
                cellRange.Text = currentEmployee.quantity.ToString();


            }
            Supplies maxSalary = Wholesale_baseEntities.GetContext().Supplies
                .OrderByDescending(p => p.delivery_price).FirstOrDefault();
            if (maxSalary != null)
            {
                Word.Paragraph maxSalaryParagraph = document.Paragraphs.Add();
                Word.Range maxSalaryRange = maxSalaryParagraph.Range;
                maxSalaryRange.Text = $"Самая дорогая управляющая компания - {maxSalary.delivery_price}";
                maxSalaryRange.Font.Color = Word.WdColor.wdColorDarkRed;
                maxSalaryRange.InsertParagraphAfter();
            }

            Supplies minSalary = Wholesale_baseEntities.GetContext().Supplies
                .OrderBy(p => p.delivery_price).FirstOrDefault();
            if (minSalary != null)
            {
                Word.Paragraph minSalaryParagraph = document.Paragraphs.Add();
                Word.Range minSalaryRange = minSalaryParagraph.Range;
                minSalaryRange.Text = $"Самая дешевая управляющая компания - {minSalary.delivery_price}";
                minSalaryRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                minSalaryRange.InsertParagraphAfter();
            }

            application.Visible = true;

            document.SaveAs2(@"C:\Users\bpvla\Desktop\270923 глеб\Wholesale base\Wholesale base\bin\Debug\Test.pdf", Word.WdExportFormat.wdExportFormatPDF);

        }
    }
}
