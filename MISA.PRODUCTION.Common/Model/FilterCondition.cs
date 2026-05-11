using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Model
{
    /// <summary>
    /// 1 điều kiện lọc trên 1 cột
    /// </summary>
    public class FilterCondition
    {
        /// <summary>
        /// Tên cột: ProductionShiftCode, WorkHour, CreatedDate, ...
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Toán tử: contains, not_contains, starts_with, ends_with, 
        /// equals, not_equals, less_than, less_than_or_equal, greater_than, greater_than_or_equal
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Giá trị lọc
        /// </summary>
        public string? Value { get; set; }
    }
}
