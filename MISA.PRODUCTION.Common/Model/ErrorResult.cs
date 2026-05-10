using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Model
{
    /// <summary>
    ///  Form mẫu trả về khi có lỗi xảy ra, có thể là lỗi validate (400), lỗi server (500), lỗi không tìm thấy (404),...
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        ///  Thông điệp lỗi dành cho developer
        /// </summary>
        public string? DevMsg { get; set; } 

        /// <summary>
        ///  Thông điệp lỗi dành cho người dùng
        /// </summary>
        /// 
        public string? UserMsg { get; set; }

        /// <summary>
        /// thông tin chi tiết hơn về lỗi
        /// </summary>
        public object? MoreInfo { get; set; }

        /// <summary>
        ///  Mã định danh duy nhất cho lỗi, có thể dùng để tra cứu log hoặc hỗ trợ debug
        /// </summary>
        public Guid? TraceId { get; set; }
    }
}
