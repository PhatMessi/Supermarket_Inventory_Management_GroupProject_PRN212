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
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Supermarket_Inventory_Management.Data;
using Supermarket_Inventory_Management.Models;

namespace Supermarket_Inventory_Management
{
    public partial class SuplierWindow : Window
    {
        public SuplierWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            dgNCC.ItemsSource = DataProvider.Context.Supliers.ToList();
        }

        private void AddSup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var ncc = new Suplier
                {
                    DisplayName = txtTen.Text.Trim(),
                    Address = txtDiaChi.Text.Trim(),
                    Phone = txtDienThoai.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    MoreInfo = txtThongTinThem.Text.Trim(),
                    ContractDate = dpNgayHopTac.SelectedDate ?? DateTime.Now
                };

                DataProvider.Context.Supliers.Add(ncc);
                DataProvider.Context.SaveChanges();
                MessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhà cung cấp.\n\nChi tiết: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditSup_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgNCC.SelectedItem is not Suplier selected)
            {
                MessageBox.Show("Vui lòng chọn 1 nhà cung cấp để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } 
                

            try
            {
                var ncc = DataProvider.Context.Supliers.Find(selected.Id);
                if (ncc != null)
                {
                    ncc.DisplayName = txtTen.Text.Trim();
                    ncc.Address = txtDiaChi.Text.Trim();
                    ncc.Phone = txtDienThoai.Text.Trim();
                    ncc.Email = txtEmail.Text.Trim();
                    ncc.MoreInfo = txtThongTinThem.Text.Trim();
                    ncc.ContractDate = dpNgayHopTac.SelectedDate ?? DateTime.Now;

                    DataProvider.Context.SaveChanges();
                    MessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhà cung cấp.\n\nChi tiết: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadEdit(Suplier selected)
        {
            txtTen.Text = selected.DisplayName;
            txtDiaChi.Text = selected.Address;
            txtDienThoai.Text = selected.Phone;
            txtEmail.Text = selected.Email;
            txtThongTinThem.Text = selected.MoreInfo;
            dpNgayHopTac.SelectedDate = selected.ContractDate;
        }
        private void dgNCC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNCC.SelectedItem is Suplier selected)
            {
                LoadEdit(selected);
            }
        }


        private void DeleteSup_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgNCC.SelectedItem is not Suplier selected)
            {
                MessageBox.Show("Vui lòng chọn 1 Nhà cung cấp để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    

            var confirm = MessageBox.Show("Bạn chắc chắn muốn xoá nhà cung cấp này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var ncc = DataProvider.Context.Supliers.Find(selected.Id);
                string suplierId= selected.Id.ToString();
                string sql = $@"
                        BEGIN TRANSACTION;
						
                            DELETE FROM OutputInfo WHERE IdInputInfo in(select id from InputInfo where(IdObject in( Select Id from Object where IdSuplier='{suplierId}')));
                            DELETE FROM InputInfo WHERE IdObject in( Select Id from Object where IdSuplier='{suplierId}');
							delete From Object Where IdSuplier = ' {suplierId}';
							delete from Suplier where Id ='{suplierId}';
                            
         
                        COMMIT;";
                DataProvider.Context.Database.ExecuteSqlRaw(sql);
                MessageBox.Show("Xóa nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa nhà cung cấp vì dữ liệu đang được sử dụng ở nơi khác.\n\nChi tiết lỗi:\n" + ex.Message,
                                "Lỗi khi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFields()
        {
            txtTen.Clear();
            txtDiaChi.Clear();
            txtDienThoai.Clear();
            txtEmail.Clear();
            txtThongTinThem.Clear();
            dpNgayHopTac.SelectedDate = null;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
}
