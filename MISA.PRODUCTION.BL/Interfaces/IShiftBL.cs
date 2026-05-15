using MISA.PRODUCTION.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.BL.Interfaces
{
    public interface IShiftBL : IBaseBL<ProductionShift>
    {
        // Hàm nhân bản bản ghi
        Task<ProductionShift> DuplicateShift(Guid id);

        // Hàm bật/tắt trạng thái ca làm việc: sử dụng, ngung sử dụng
        Task<int> ToggleStatus(List<Guid> ids, int status);

        // Tạo Excel
        Task<byte[]> ExportExcel(FilterPagingRequest request);
    }
}
