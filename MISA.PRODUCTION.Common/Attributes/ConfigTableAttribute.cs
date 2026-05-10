using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Attributes
{
    /// <summary>
    /// Cấu hình tên bảng trong database cho class model
    /// Phòng TH nếu tên bảng trong database khác với tên class model thì sẽ sử dụng attribute này để cấu hình tên bảng
    /// </summary>
    public class ConfigTableAttribute : Attribute
    {
        public string TableName { get; set; }

        public ConfigTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
