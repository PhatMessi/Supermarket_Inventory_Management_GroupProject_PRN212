
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Supermarket_Inventory_Management.Data;
using Supermarket_Inventory_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls.Primitives;

namespace Supermarket_Inventory_Management
{
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
            LoadComboBox();
            LoadData();
        }

        private void LoadComboBox()
        {
            cbVatTu.ItemsSource = DataProvider.Context.Objects.ToList();
        }

        private void LoadData()
        {
            var list = DataProvider.Context.InputInfos
                .Include(ii => ii.IdObjectNavigation)
                .Include(ii => ii.IdInputNavigation)
                .ToList();

            dgNhapKho.ItemsSource = list;
        }


        private void AddInput_Click(object sender, RoutedEventArgs e)
        {
            if (cbVatTu.SelectedItem is not Supermarket_Inventory_Management.Models.Object selectedObj)
            {
                MessageBox.Show("Vui lòng nhập tên Sản phẩm nhập kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var input = new Input
                {
                    Id = Guid.NewGuid().ToString(),
                    DateInput = dpNgayNhap.SelectedDate ?? DateTime.Now
                };

                var inputInfo = new InputInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    IdInput = input.Id,
                    IdObject = selectedObj.Id,
                    Count = int.TryParse(txtSoLuong.Text, out int soLuong) ? soLuong : 0,
                    InputPrice = double.TryParse(txtGiaNhap.Text, out double giaNhap) ? giaNhap : 0,
                    OutputPrice = double.TryParse(txtGiaXuat.Text, out double giaXuat) ? giaXuat : 0,
                    Status = txtTrangThai.Text
                };

                DataProvider.Context.Inputs.Add(input);
                DataProvider.Context.InputInfos.Add(inputInfo);
                DataProvider.Context.SaveChanges();

                MessageBox.Show("Thêm nhập kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm nhập kho.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditInput_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgNhapKho.SelectedItem is not InputInfo selected)
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                

            try
            {
                var inputInfo = DataProvider.Context.InputInfos
                    .Include(ii => ii.IdObjectNavigation)
                    .Include(ii => ii.IdInputNavigation)
                    .FirstOrDefault(ii => ii.Id == selected.Id);

                if (inputInfo != null)
                {
                    if (int.TryParse(txtSoLuong.Text, out int count))
                        inputInfo.Count = count;

                    if (double.TryParse(txtGiaNhap.Text, out double inputprice))
                        inputInfo.InputPrice = inputprice;

                    if (double.TryParse(txtGiaXuat.Text, out double outputprice))
                        inputInfo.OutputPrice = outputprice;

                    inputInfo.Status = txtTrangThai.Text;

                    DataProvider.Context.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin nhập kho thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi sửa nhập kho.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEdit(InputInfo currentinput)
        {

            txtSoLuong.Text = currentinput.Count.ToString();
            txtGiaNhap.Text = currentinput.InputPrice.ToString();
            txtGiaXuat.Text = currentinput.OutputPrice.ToString();
            txtTrangThai.Text = currentinput.Status;

            if (currentinput.IdInputNavigation != null)
            {
                dpNgayNhap.SelectedDate = currentinput.IdInputNavigation.DateInput;
            }

            if (currentinput.IdObjectNavigation != null)
            {
                cbVatTu.SelectedItem = currentinput.IdObjectNavigation;
            }
        }
        private void dgNhapKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNhapKho.SelectedItem is InputInfo select)
            {
                LoadEdit(select);
            }
        }
        private void DeleteInput_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgNhapKho.SelectedItem is not InputInfo selectedInputInfo)
            {
                MessageBox.Show("Vui lòng chọn 1 sản phẩm để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi nhập kho này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var input = DataProvider.Context.Inputs
                    .FirstOrDefault(i => i.Id == selectedInputInfo.IdInput);

                DataProvider.Context.InputInfos.Remove(selectedInputInfo);
                if (input != null)
                {
                    DataProvider.Context.Inputs.Remove(input);
                }

                DataProvider.Context.SaveChanges();
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa bản ghi vì dữ liệu này đang được sử dụng ở nơi khác (ví dụ như trong xuất kho).\n\nChi tiết lỗi:\n" + ex.Message,
                    "Lỗi khi xóa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        private void ClearFields()
        {
            txtSoLuong.Clear();
            txtGiaNhap.Clear();
            txtGiaXuat.Clear();
            txtTrangThai.Clear();
            cbVatTu.SelectedIndex = -1;
            dpNgayNhap.SelectedDate = null;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
}

