using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Extension
{
    public static class SqlFilterBuilder
    {
        /// <summary>
        /// Build mệnh đề WHERE từ Keyword + Filters
        /// </summary>
        public static string BuildWhereClause<T>(
            string? keyword, 
            List<Model.FilterCondition>? filters,
            ref DynamicParameters param)
        {
            var type = typeof(T);
            var sqlWhere = new StringBuilder(" WHERE 1 = 1 "); // đỡ phải viết AND đầu tiên
            int paramIndex = 0;

            // 1. Keyword: tìm kiếm trên tất cả cột string
            var stringColumns = type.GetStringColumns(); // danh sách cột kiểu string
            if (!string.IsNullOrWhiteSpace(keyword) && stringColumns.Any())
            {
                sqlWhere.Append(" AND (");
                for (int i = 0; i < stringColumns.Count; i++)
                {
                    if (i > 0) sqlWhere.Append(" OR "); // Từ cột thứ 2 trở đi thì thêm chữ OR vào trước
                    sqlWhere.Append($"`{stringColumns[i]}` LIKE @Keyword");
                }
                sqlWhere.Append(")");
                param.Add("@Keyword", $"%{keyword.Trim()}%");
                //sqlWhere = WHERE 1=1 AND (ProductCode LIKE '%MISA%' OR ProductName LIKE '%MISA%')
            }

            // 2. Filters: lọc theo từng cột
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (string.IsNullOrWhiteSpace(filter.Property) || string.IsNullOrWhiteSpace(filter.Operator))
                        continue;

                    // Kiểm tra property có tồn tại trên model không
                    //var prop = type.GetProperty(filter.Property);
                    var prop = type.GetProperties()
                            .FirstOrDefault(p => string.Equals(p.Name, filter.Property, StringComparison.OrdinalIgnoreCase));

                    if (prop == null) continue;

                    // Tạo tên tham số động để tránh trùng lặp
                    var paramName = $"@Filter_{paramIndex}";

                    // Chuẩn hóa operator về chữ thường để dễ so sánh
                    var op = filter.Operator.Trim().ToLower();

                    switch (op)
                    {
                        // === String operators ===
                        case "contains":
                            sqlWhere.Append($" AND `{filter.Property}` LIKE {paramName}");
                            param.Add(paramName, $"%{filter.Value}%");
                            break;

                        case "not_contains":
                            sqlWhere.Append($" AND `{filter.Property}` NOT LIKE {paramName}");
                            param.Add(paramName, $"%{filter.Value}%");
                            break;

                        case "starts_with":
                            sqlWhere.Append($" AND `{filter.Property}` LIKE {paramName}");
                            param.Add(paramName, $"{filter.Value}%");
                            break;

                        case "ends_with":
                            sqlWhere.Append($" AND `{filter.Property}` LIKE {paramName}");
                            param.Add(paramName, $"%{filter.Value}");
                            break;

                        // === So sánh (dùng cho number, date, string) ===
                        case "equals":
                            sqlWhere.Append($" AND `{filter.Property}` = {paramName}");
                            param.Add(paramName, filter.Value);
                            break;

                        case "not_equals":
                            sqlWhere.Append($" AND `{filter.Property}` <> {paramName}");
                            param.Add(paramName, filter.Value);
                            break;

                        case "less_than":
                            sqlWhere.Append($" AND `{filter.Property}` < {paramName}");
                            param.Add(paramName, filter.Value);
                            break;

                        case "less_than_or_equal":
                            sqlWhere.Append($" AND `{filter.Property}` <= {paramName}");
                            param.Add(paramName, filter.Value);
                            break;

                        case "greater_than":
                            sqlWhere.Append($" AND `{filter.Property}` > {paramName}");
                            param.Add(paramName, filter.Value);
                            break;

                        case "greater_than_or_equal":
                            sqlWhere.Append($" AND `{filter.Property}` >= {paramName}");
                            param.Add(paramName, filter.Value);
                            break;
                    }

                    paramIndex++;
                }
            }

            //WHERE 1 = 1
            //     AND(`ProductCode` LIKE @Keyword OR `ProductName` LIKE @Keyword)
            //     AND `ProductName` LIKE @Filter_0
            //     AND `Price` >= @Filter_1

            return sqlWhere.ToString();
        }

        /// <summary>
        /// Build mệnh đề ORDER BY
        /// sortBy: tên cột muốn sắp xếp
        /// sortDirection: hướng sắp xếp (ASC hoặc DESC)
        /// </summary>
        //public static string BuildOrderByClause<T>(string? sortBy, string? sortDirection)
        //{
        //    // mặc định sắp xếp theo ngày tạo giảm dần-> mới nhất lên đầu
        //    if (string.IsNullOrWhiteSpace(sortBy)) return " ORDER BY CreatedDate DESC";

        //    // Kiểm tra cột có tồn tại không
        //    var prop = typeof(T).GetProperty(sortBy);
        //    if (prop == null) return " ORDER BY CreatedDate DESC";

        //    var direction = string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
        //        ? "DESC" : "ASC";

        //    var res = $" ORDER BY `{sortBy}` {direction}";

        //    return res; //ORDER BY CreatedDate DESC
        //}

        public static string BuildOrderByClause<T>(string? sortBy, string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy)) return " ORDER BY CreatedDate DESC";

            // ★ Case-insensitive lookup (giống BuildWhereClause)
            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase));
            if (prop == null) return " ORDER BY CreatedDate DESC";

            var direction = string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC" : "ASC";

            // ★ Dùng prop.Name (đúng case) thay vì sortBy từ frontend
            return $" ORDER BY `{prop.Name}` {direction}";
        }
    }
}
