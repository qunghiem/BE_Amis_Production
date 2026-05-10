using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Extension
{
    /// <summary>
    /// Exception cho lỗi validate nghiệp vụ (trả về 400: lỗi ng dùng)
    /// vd: mã ca trống, email sai định dạng, ngày sinh > ngày hiện tại,...
    /// </summary>
    public class ValidateException : Exception
    {
        /// <summary>
        /// Chứa danh sách lỗi
        /// Exception thông thường chỉ có 1 message, nhưng có thể có nhiều lỗi validate cùng lúc, nên cần 1 list để chứa tất cả lỗi
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Khởi tạo exception với 1 lỗi
        /// </summary>
        /// <param name="message">Thông điệp lỗi</param>
        public ValidateException(string message) : base(message) 
        {
            Errors = new List<string> { message };
        }

        /// <summary>
        /// Khởi tạo exception với nhiều lỗi
        /// </summary>
        /// <param name="errors">Danh sách thông điệp lỗi</param>
        public ValidateException(List<string> errors) : base(string.Join("; ", errors))
        {
            Errors = errors;
        }
    }
}
