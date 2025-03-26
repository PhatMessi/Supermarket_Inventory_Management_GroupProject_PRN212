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
    public partial class ObjectWindow : Window
    {
        public ObjectWindow()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            cbDonViDo.ItemsSource = DataProvider.Context.Units.ToList();
            cbNhaCungCap.ItemsSource = DataProvider.Context.Supliers.ToList();
        }

        private void LoadData()
        {
            var list = DataProvider.Context.Objects
                .Include(o => o.IdUnitNavigation)
                .Include(o => o.IdSuplierNavigation)
                .ToList();

            dgVatTu.ItemsSource = list;
        }


        private void AddObj_Click(object sender, RoutedEventArgs e)
        {
            
            if (cbDonViDo.SelectedItem is not Unit selectedUnit || cbNhaCungCap.SelectedItem is not Suplier selectedSuplier)
            {
                MessageBox.Show("Vui lòng nhập tên Vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var obj = new Models.Object
                {
                    Id = Guid.NewGuid().ToString(),
                    DisplayName = txtTenVatTu.Text.Trim(),
                    IdUnit = selectedUnit.Id,
                    IdSuplier = selectedSuplier.Id,
                    Qrcode = txtQRCode.Text.Trim(),
                    BarCode = txtBarCode.Text.Trim()
                };

                DataProvider.Context.Objects.Add(obj);
                DataProvider.Context.SaveChanges();
                MessageBox.Show("Thêm vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vật tư.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditObj_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgVatTu.SelectedItem is not Models.Object selectedObj)
            {
                MessageBox.Show("Vui lòng chọn 1 Vật tư để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            try
            {
                if (cbDonViDo.SelectedItem is Unit selectedUnit)
                    selectedObj.IdUnit = selectedUnit.Id;

                if (cbNhaCungCap.SelectedItem is Suplier selectedSuplier)
                    selectedObj.IdSuplier = selectedSuplier.Id;

                selectedObj.DisplayName = txtTenVatTu.Text.Trim();
                selectedObj.Qrcode = txtQRCode.Text.Trim();
                selectedObj.BarCode = txtBarCode.Text.Trim();

                DataProvider.Context.SaveChanges();
                MessageBox.Show("Cập nhật vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật vật tư.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void dgVatTu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVatTu.SelectedItem is Supermarket_Inventory_Management.Models.Object selectedObj)
            {
                txtTenVatTu.Text = selectedObj.DisplayName;
                txtQRCode.Text = selectedObj.Qrcode;
                txtBarCode.Text = selectedObj.BarCode;

                cbDonViDo.SelectedItem = selectedObj.IdUnitNavigation;
                cbNhaCungCap.SelectedItem = selectedObj.IdSuplierNavigation;
            }
        }

        private void DeleteOjb_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgVatTu.SelectedItem is not Models.Object selectedObj)
            {
                MessageBox.Show("Vui lòng chọn 1 Vật tư để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    
                

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa vật tư này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                DataProvider.Context.Objects.Remove(selectedObj);
                DataProvider.Context.SaveChanges();
                MessageBox.Show("Xóa vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa vật tư này vì nó đang được sử dụng ở nơi khác.\n\nChi tiết lỗi:\n" + ex.Message,
                                "Lỗi khi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFields()
        {
            txtTenVatTu.Clear();
            txtQRCode.Clear();
            txtBarCode.Clear();
            cbDonViDo.SelectedIndex = -1;
            cbNhaCungCap.SelectedIndex = -1;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
}
