using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Supermarket_Inventory_Management.Data;
using Supermarket_Inventory_Management.Models;

namespace Supermarket_Inventory_Management
{
    public partial class UnitWindow : Window
    {
        public UnitWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            dgDonViDo.ItemsSource = DataProvider.Context.Units.ToList();
        }

        private void AddUnit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenDonViDo.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị đo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dvi = new Unit { DisplayName = txtTenDonViDo.Text.Trim() };
                DataProvider.Context.Units.Add(dvi);
                DataProvider.Context.SaveChanges();
                MessageBox.Show("Thêm đơn vị đo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                txtTenDonViDo.Clear();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm đơn vị đo.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditUnit_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgDonViDo.SelectedItem is not Unit selected)
            {
                MessageBox.Show("Vui lòng chọn 1 đơn vị đo để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            if (string.IsNullOrWhiteSpace(txtTenDonViDo.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị đo để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dvi = DataProvider.Context.Units.Find(selected.Id);
                if (dvi != null)
                {
                    dvi.DisplayName = txtTenDonViDo.Text.Trim();
                    DataProvider.Context.SaveChanges();
                    MessageBox.Show("Cập nhật đơn vị đo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    txtTenDonViDo.Clear();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật đơn vị đo.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadEdit(Unit currentunit)
        {

            txtTenDonViDo.Text = currentunit.DisplayName.ToString();
        }
        private void dgDonViDo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDonViDo.SelectedItem is Unit select)
            {
                LoadEdit(select);
            }
        }

        private void DeleteUnit_Click(object sender, RoutedEventArgs e)
        {
           
            if (dgDonViDo.SelectedItem is not Unit selected)
            {
                MessageBox.Show("Vui lòng chọn 1 đơn vị đo để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa đơn vị đo này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var dvi = DataProvider.Context.Units.Find(selected.Id);
                string dvid = selected.Id.ToString();
                string sql = $@"
                        BEGIN TRANSACTION;
						
                            DELETE FROM OutputInfo WHERE IdInputInfo in(select id from InputInfo where(IdObject in( Select Id from Object where IdUnit='{dvid}')));
                            DELETE FROM InputInfo WHERE IdObject in( Select Id from Object where IdUnit='{dvid}');
							delete From Object Where IdUnit = ' {dvid}';
							delete from Unit where Id ='{dvid}';
                            
         
                        COMMIT;";
                DataProvider.Context.Database.ExecuteSqlRaw(sql);
                MessageBox.Show("Xóa đơn vị đo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                   
                    ClearFields();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa đơn vị đo này vì đang được sử dụng ở nơi khác.\n\nChi tiết lỗi:\n" + ex.Message,
                                "Lỗi khi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFields()
        {
            txtTenDonViDo.Clear();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
}
