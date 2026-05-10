using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Enums
{
    /// <summary>
    /// Enum quản lý trạng thái của model
    /// </summary>
    public enum ModelState : int
    {
        /// <summary>
        /// Không làm gì cả, trạng thái mặc định của model, không có sự thay đổi nào được thực hiện
        /// giá trị nào = 0 thì sẽ được coi là trạng thái mặc định
        /// </summary>
        None = 0,

        /// <summary>
        /// Thêm mới một bản ghi
        /// </summary>
        Insert = 1,

        /// <summary>
        /// Cập nhật một bản ghi
        /// </summary>
        Update = 2,

        /// <summary>
        /// Xóa một bản ghi
        /// </summary>
        Delete = 3
    }
}
