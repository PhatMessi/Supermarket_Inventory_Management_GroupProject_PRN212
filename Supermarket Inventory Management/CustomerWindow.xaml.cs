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
using Supermarket_Inventory_Management.Data;
using Supermarket_Inventory_Management.Models;

namespace Supermarket_Inventory_Management
{
    public partial class CustomerWindow : Window
    {
        public CustomerWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var list = DataProvider.Context.Customers.ToList();
            dgKH.ItemsSource = list;
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                MessageBox.Show("Vui lòng nhập tên Khách hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var kh = new Customer
                {
                    DisplayName = txtTen.Text,
                    Address = txtDiaChi.Text,
                    Phone = txtDienThoai.Text,
                    Email = txtEmail.Text,
                    MoreInfo = txtThongTinThem.Text,
                    ContractDate = dpNgayHopTac.SelectedDate ?? DateTime.Now
                };

                DataProvider.Context.Customers.Add(kh);
                DataProvider.Context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {            
            if (dgKH.SelectedItem is not Customer selected)
            {
                MessageBox.Show("Vui lòng chọn 1 Khách hàng để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } 
                

            try
            {
                var kh = DataProvider.Context.Customers.Find(selected.Id);
                if (kh != null)
                {
                    kh.DisplayName = txtTen.Text;
                    kh.Address = txtDiaChi.Text;
                    kh.Phone = txtDienThoai.Text;
                    kh.Email = txtEmail.Text;
                    kh.MoreInfo = txtThongTinThem.Text;
                    kh.ContractDate = dpNgayHopTac.SelectedDate ?? DateTime.Now;

                    DataProvider.Context.SaveChanges();
                    LoadData();
                    ClearFields();

                    MessageBox.Show("Cập nhật thông tin khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật khách hàng:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadEdit(Customer currentcustomer)
        {
            txtTen.Text = currentcustomer.DisplayName;
            txtDiaChi.Text = currentcustomer.Address;
            txtDienThoai.Text = currentcustomer.Phone;
            txtEmail.Text = currentcustomer.Email;
            txtThongTinThem.Text = currentcustomer.MoreInfo;
            dpNgayHopTac.SelectedDate = currentcustomer.ContractDate;
        }
        private void dgKH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKH.SelectedItem is Customer select)
            {
                LoadEdit(select);
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgKH.SelectedItem is not Customer selected)
            {
                MessageBox.Show("Vui lòng chọn 1 Khách hàng để Xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    

            var confirm = MessageBox.Show("Xác nhận xoá khách hàng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var kh = DataProvider.Context.Customers.Find(selected.Id);
                if (kh != null)
                {
                    DataProvider.Context.Customers.Remove(kh);
                    DataProvider.Context.SaveChanges();
                    LoadData();
                    ClearFields();

                    MessageBox.Show("Xoá khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xoá khách hàng:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
