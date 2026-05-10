using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Attributes
{
    /// <summary>
    /// Tạo attribute để đánh dấu các property cần kiểm tra trùng lặp khi thêm mới hoặc cập nhật
    /// </summary>

    
    [AttributeUsage(AttributeTargets.Property)] // Chỉ áp dụng cho các thuộc tính
    public class CheckDuplicateAttribute : Attribute
    {
        // Thuộc tính để lưu thông báo lỗi khi có dữ liệu trùng lặp
        public string ErrorMessage { get; set; }

        // khởi tạo truyền vào thông báo lỗi khi có dữ liệu trùng lặp
        public CheckDuplicateAttribute(string errorMessage) 
        { 
            ErrorMessage = errorMessage;
        }
    }
}
