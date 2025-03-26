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
    public partial class OutputWindow : Window
    {
        public OutputWindow()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            cbVatTu.ItemsSource = DataProvider.Context.Objects.ToList();
            cbKhachHang.ItemsSource = DataProvider.Context.Customers.ToList();
        }

        private void LoadData()
        {
            var data = DataProvider.Context.OutputInfos
                .Include(x => x.IdObjectNavigation)
                .Include(x => x.IdCustomerNavigation)
                .Include(x => x.IdInputInfoNavigation)
                .ThenInclude(ii => ii.IdInputNavigation)
                .ToList();

            dgXuatKho.ItemsSource = data;
        }

        private void AddOut_Click(object sender, RoutedEventArgs e)
        {
            
            if (cbVatTu.SelectedItem is not Models.Object selectedObj || cbKhachHang.SelectedItem is not Customer selectedCus)
            {
                MessageBox.Show("Vui lòng nhập tên Sản phẩm xuất kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            try
            {
                var inputInfo = selectedObj.InputInfos.FirstOrDefault();
                if (inputInfo == null)
                {
                    MessageBox.Show("Vật tư chưa có dữ liệu nhập kho, không thể xuất kho!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int tongSoLuongNhap = DataProvider.Context.InputInfos
                    .Where(ii => ii.IdObject == selectedObj.Id)
                    .Sum(ii => ii.Count ?? 0);

                int tongSoLuongXuat = DataProvider.Context.OutputInfos
                    .Where(oi => oi.IdObject == selectedObj.Id)
                    .Sum(oi => oi.Count ?? 0);

                int soLuongTonKho = tongSoLuongNhap - tongSoLuongXuat;

                if (!int.TryParse(txtSoLuong.Text, out int soLuongXuat) || soLuongXuat <= 0)
                {
                    MessageBox.Show("Số lượng xuất không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (soLuongXuat > soLuongTonKho)
                {
                    MessageBox.Show($"Số lượng xuất ({soLuongXuat}) vượt quá số lượng tồn kho ({soLuongTonKho})!",
                                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var outputInfo = new OutputInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    IdObject = selectedObj.Id,
                    IdInputInfo = inputInfo.Id,
                    IdCustomer = selectedCus.Id,
                    Count = soLuongXuat,
                    Status = txtTrangThai.Text
                };

                DataProvider.Context.OutputInfos.Add(outputInfo);
                DataProvider.Context.SaveChanges();

                MessageBox.Show("Xuất kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm xuất kho.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditOut_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgXuatKho.SelectedItem is not OutputInfo selected)
            {
                MessageBox.Show("Vui lòng chọn 1 Sản phẩm để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            try
            {
                selected.Count = int.TryParse(txtSoLuong.Text, out int sl) ? sl : selected.Count;
                selected.Status = txtTrangThai.Text;
                selected.IdCustomer = (cbKhachHang.SelectedItem as Customer)?.Id ?? selected.IdCustomer;
                selected.IdInputInfoNavigation.OutputPrice = double.TryParse(txtGiaXuat.Text, out double gx) ? gx : selected.IdInputInfoNavigation.OutputPrice;

                DataProvider.Context.SaveChanges();
                MessageBox.Show("Cập nhật xuất kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật xuất kho.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadEdit(OutputInfo current)
        {
            txtSoLuong.Text = current.Count?.ToString();
            txtGiaXuat.Text = current.IdInputInfoNavigation.OutputPrice?.ToString();
            txtTrangThai.Text = current.Status;
            dpNgayXuat.SelectedDate = current.IdInputInfoNavigation.IdInputNavigation?.DateInput;
            cbVatTu.SelectedItem = current.IdObjectNavigation;
            cbKhachHang.SelectedItem = current.IdCustomerNavigation;
        }
        private void dgXuatKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgXuatKho.SelectedItem is OutputInfo selected)
            {
                LoadEdit(selected);
            }
        }

        private void DeleteOut_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgXuatKho.SelectedItem is not OutputInfo selected)
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa dòng xuất kho này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                DataProvider.Context.OutputInfos.Remove(selected);
                DataProvider.Context.SaveChanges();
                MessageBox.Show("Xóa xuất kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa dữ liệu xuất kho.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFields()
        {
            txtSoLuong.Clear();
            txtGiaXuat.Clear();
            txtTrangThai.Clear();
            cbVatTu.SelectedIndex = -1;
            cbKhachHang.SelectedIndex = -1;
            dpNgayXuat.SelectedDate = null;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
}
