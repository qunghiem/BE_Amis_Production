using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.BL.Interfaces
{
    public interface IBaseBL<T>
    {
        // Lấy bản ghi theo id
        Task<T> GetById(Guid id);
    }
}
