using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Resources
{
    /// <summary>
    /// chứa các chuỗi thông báo lỗi, thành công, cảnh báo,... bằng tiếng Việt để sử dụng trong toàn bộ dự án
    /// </summary>
    public static class ResourceVN
    {
        public static string Exception = "Có lỗi xảy ra, vui lòng liên hệ MISA!";
        public static string NotFoundPrimaryKey = "Không tìm thấy khóa chính của model";
        public static string NotFound = "Dữ liệu không tồn tại trên hệ thống";
    }
}
