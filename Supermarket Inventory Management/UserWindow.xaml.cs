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
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            LoadComboBox();
            LoadData();
        }

        private void LoadComboBox()
        {
            cbRole.ItemsSource = DataProvider.Context.UserRoles.ToList();
            cbRole.DisplayMemberPath = "DisplayName";
            cbRole.SelectedValuePath = "Id";
        }


        private void LoadData()
        {
            var list = DataProvider.Context.Users
                .Include(x => x.IdRoleNavigation)
                .ToList();

            dgUser.ItemsSource = list;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtDisplayName.Text) || cbRole.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin người dùng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = new User
                {
                    UserName = txtUserName.Text.Trim(),
                    DisplayName = txtDisplayName.Text.Trim(),
                    IdRole = (int)cbRole.SelectedValue,
                    Password = HashPassword("123")
                };

                DataProvider.Context.Users.Add(user);
                DataProvider.Context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm người dùng.\n\nChi tiết lỗi:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgUser.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn 1 Người dùng để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    

            try
            {
                var userId = (int)dgUser.SelectedItem.GetType().GetProperty("Id")!.GetValue(dgUser.SelectedItem);
                var user = DataProvider.Context.Users.Find(userId);

                if (user != null)
                {
                    user.UserName = txtUserName.Text.Trim();
                    user.DisplayName = txtDisplayName.Text.Trim();
                    user.IdRole = (int)cbRole.SelectedValue;

                    DataProvider.Context.SaveChanges();
                    LoadData();
                    ClearFields();

                    MessageBox.Show("Cập nhật người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật người dùng:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadEdit(User currentUser)
        {
            txtUserName.Text = currentUser.UserName;
            txtDisplayName.Text = currentUser.DisplayName;

            if (currentUser.IdRoleNavigation != null)
            {
                cbRole.SelectedItem = currentUser.IdRoleNavigation;
            }
        }
        private void dgUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUser.SelectedItem is User selectedUser)
            {
                LoadEdit(selectedUser);
            }
        }
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgUser.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn 1 Người dùng để Xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }    

            var confirm = MessageBox.Show("Xoá người dùng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var id = (int)dgUser.SelectedItem.GetType().GetProperty("Id")!.GetValue(dgUser.SelectedItem);
                var user = DataProvider.Context.Users.Find(id);

                if (user != null)
                {
                    DataProvider.Context.Users.Remove(user);
                    DataProvider.Context.SaveChanges();
                    LoadData();
                    ClearFields();

                    MessageBox.Show("Xoá người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xoá người dùng:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangPassUser_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUser.SelectedItem;
            if (selected == null) return;

            var id = (int)selected.GetType().GetProperty("Id").GetValue(selected);
            var user = DataProvider.Context.Users.Find(id);
            user.Password = HashPassword("123"); 
            DataProvider.Context.SaveChanges();

            MessageBox.Show("Đã đặt lại mật khẩu thành '123'", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string HashPassword(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(bytes);
                return string.Concat(hashBytes.Select(b => b.ToString("x2")));
            }
        }
        private void ClearFields()
        {
            txtUserName.Clear();
            txtDisplayName.Clear();
            cbRole.SelectedIndex = -1;
        }
    }
}
