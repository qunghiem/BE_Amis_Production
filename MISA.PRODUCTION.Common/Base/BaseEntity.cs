using MISA.PRODUCTION.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Base
{
    /// <summary>
    /// Các trường thông tin chung của tất cả các entity trong hệ thống, bao gồm:
    /// CreatedBy: Người tạo
    /// CreatedDate: Ngày tạo
    /// ModifiedBy: Người sửa
    /// ModifiedDate: Ngày sửa
    /// </summary>
    public class BaseEntity
    {
        // Người tạo
        public string? CreatedBy { get; set; } = "Hệ thống tự động";

        // Ngày tạo, mặc định là ngày hiện tại khi khởi tạo đối tượng
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Người sửa
        public string? ModifiedBy { get; set; }

        // Ngày sửa, mặc định là ngày hiện tại khi khởi tạo đối tượng
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // Thuộc tính này không được ánh xạ vào database, chỉ dùng để quản lý trạng thái của model trong quá trình xử lý nghiệp vụ
        [NotMapped]
        public ModelState ModelState { get; set; }
    }
}
