using MISA.PRODUCTION.Common.Model;
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

        // Hàm thêm mới bản ghi
        Task<int> Insert(T entity);

        // Hàm cập nhật bản ghi
        Task<int> Update(T entity);

        // Hàm xóa bản ghi: 1 hoặc nhiều bản ghi cùng lúc
        Task<int> Delete(List<Guid> ids);
        
        // Hàm tìm kiếm có phân trang kết hợp lọc nhiều điều kiện, sắp xếp
        Task<PagingResult<T>> GetFilterPaging(FilterPagingRequest request);
    }
}
