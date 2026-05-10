using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.DL.Interfaces
{
    public interface IBaseDL<T>
    {
        // Lấy bản ghi theo ID
        Task<T> GetById(Guid id);
    }
}
