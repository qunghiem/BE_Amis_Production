using MISA.PRODUCTION.Common.Model;
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

        // Hàm thêm mới bản ghi
        Task<int> Insert(T entity);

        // Hàm kiểm tra trùng lặp dữ liệu
        Task<bool> CheckDuplicate(string propName, object value, Guid? excludeId);

        // Hàm cập nhật bản ghi
        Task<int> Update(T entity);

        // Hàm xóa bản ghi: 1 hoặc nhiều bản ghi cùng lúc
        Task<int> Delete(List<Guid> ids);
        
        // Hàm lấy dữ liệu có phân trang và lọc
        Task<PagingResult<T>> GetFilterPaging(FilterPagingRequest request);
    }
}
