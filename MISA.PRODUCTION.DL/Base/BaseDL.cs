using MISA.PRODUCTION.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.DL.Base
{
    public class BaseDL<T> : IBaseDL<T>
    {
        // Chuỗi kết nối đến database
        private string connectionString = "server=localhost;port=3306;database=misa_amisproduction;user=root;password=root;";
    
    
    }
}
