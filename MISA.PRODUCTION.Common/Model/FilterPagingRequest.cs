using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Model
{
    public class FilterPagingRequest
    {
        /// Từ khóa tìm kiếm chung (search trên tất cả cột string)
        public string? Keyword { get; set; }
         
        /// Danh sách điều kiện lọc theo từng cột
        public List<FilterCondition>? Filters { get; set; }

        /// Cột sắp xếp: ProductionShiftName, CreatedDate, ...
        public string? SortBy { get; set; }

        /// Hướng sắp xếp: ASC / DESC
        public string? SortDirection { get; set; } = "ASC";

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
