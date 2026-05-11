using MISA.PRODUCTION.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.DL.Interfaces
{
    public interface IShiftDL : IBaseDL<ProductionShift>
    {
        // Hàm chuyển đổi trạng thái của ca làm việc: Sử dụng <-> Ngừng sử dụng
        Task<int> ToggleStatus(List<Guid> ids, int status);
    }
}
