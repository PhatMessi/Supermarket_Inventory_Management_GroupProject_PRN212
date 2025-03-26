using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supermarket_Inventory_Management.Models;

namespace Supermarket_Inventory_Management.Data
{
    public class DataProvider
    {
        private static QuanLyKhoSieuThiContext _context;

        public static QuanLyKhoSieuThiContext Context
        {
            get
            {
                if (_context == null)
                    _context = new QuanLyKhoSieuThiContext();
                return _context;
            }
        }
    }
}
