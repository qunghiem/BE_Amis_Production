using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Model
{
    public class PagingResult<T>
    {
        // tổng bản ghi sau khi đã lọc, chưa phân trang
        public int TotalRecord { get; set; }

        // tổng số trang, dựa vào TotalRecord và PageSize
        public int TotalPage { get; set; }

        // số trang hiện tại
        public int PageNumber { get; set; }

        // số bản ghi trên mỗi trang    
        public int PageSize { get; set; }

        // dữ liệu sau khi đã phân trang
        public List<T> Data { get; set; } = new List<T>();
    }
}
