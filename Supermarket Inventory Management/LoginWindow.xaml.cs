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

namespace Supermarket_Inventory_Management
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        public static string GetMd5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string hashedPassword = GetMd5Hash(password);

            var user = DataProvider.Context.Users
                         .FirstOrDefault(u => u.UserName == username && u.Password == hashedPassword);

            if (user != null)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
//MessageBox.Show($"Username: {username}\nPassword: {password}\nMD5: {hashedPassword}");
