using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.PRODUCTION.Common.Enums;

namespace MISA.PRODUCTION.Common.Model
{
    /// <summary>
    /// Yêu cầu thay đổi trạng thái
    /// </summary>
    public class ToggleStatusRequest
    {
        // Danh sách Id của các bản ghi cần thay đổi trạng thái
        public List<Guid> Ids { get; set; } = new List<Guid>();
        public ShiftStatus Status { get; set; }     // 1: Sử dụng, 0: Ngừng sử dụng
    }
}
