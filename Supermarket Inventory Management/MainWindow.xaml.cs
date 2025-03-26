using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Supermarket_Inventory_Management.Data;
using Supermarket_Inventory_Management.Models;

namespace Supermarket_Inventory_Management
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTonKhoData();
            LoadThongKe();
        }

        private void NhapKho_Click(object sender, RoutedEventArgs e)
        {
            new InputWindow().Show();
        }

        private void XuatKho_Click(object sender, RoutedEventArgs e)
        {
            new OutputWindow().Show();
        }

        private void VatTu_Click(object sender, RoutedEventArgs e)
        {
            new ObjectWindow().Show();
        }

        private void DonViDo_Click(object sender, RoutedEventArgs e)
        {
            new UnitWindow().Show();
        }

        private void NhaCungCap_Click(object sender, RoutedEventArgs e)
        {
            new SuplierWindow().Show();
        }

        private void KhachHang_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow().Show();
        }

        private void NguoiDung_Click(object sender, RoutedEventArgs e)
        {
            new UserWindow().Show();
        }

        private void LoadTonKhoData()
        {
            var tonKho = DataProvider.Context.Objects
            .Select(obj => new
            {
                obj.DisplayName,
                SoLuongTon = obj.InputInfos.Sum(ii => ii.Count) - obj.OutputInfos.Sum(oi => oi.Count)
            })
            .ToList();

            var listWithIndex = tonKho.Select((x, i) => new
            {
                STT = i + 1,
                TenVatTu = x.DisplayName,
                x.SoLuongTon
            }).ToList();

            fullTonKhoList = listWithIndex.Cast<dynamic>().ToList();
            dgTonKho.ItemsSource = listWithIndex;
        }
        private List<dynamic> fullTonKhoList;

        private void LoadThongKe()
        {
            int tongNhap = DataProvider.Context.InputInfos
                .Where(ii => ii.Count.HasValue)
                .Sum(ii => ii.Count.Value);

            int tongXuat = DataProvider.Context.OutputInfos
                .Where(oi => oi.Count.HasValue)
                .Sum(oi => oi.Count.Value);

            int tonKho = tongNhap - tongXuat;

            txtNhap.Text = tongNhap.ToString();
            txtXuat.Text = tongXuat.ToString();
            txtTon.Text = tonKho.ToString();
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            var filtered = fullTonKhoList
                .Where(item => item.TenVatTu.ToLower().Contains(keyword))
                .ToList();

            dgTonKho.ItemsSource = filtered;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadThongKe();
            LoadTonKhoData();
        }
    }
}