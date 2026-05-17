using Dapper;
using MISA.PRODUCTION.Common.Extension;
using MISA.PRODUCTION.Common.Model;
using MISA.PRODUCTION.DL.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.DL.Base
{
    public class BaseDL<T> : IBaseDL<T>
    {
        // Chuỗi kết nối đến database
        protected string connectionString = "server=localhost;port=3306;database=misa_amisproduction;user=root;password=root;";

        /// <summary>
        /// Lấy bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetById(Guid id) 
        {
            // Lấy tên bảng từ kiểu dữ liệu T
            var tableName = typeof(T).GetTableNameOnly();

            // Lấy tên khóa chính từ kiểu dữ liệu T
            var primaryKeyName = typeof(T).GetPrimaryKey();

            var sql = $"SELECT * FROM `{tableName}` WHERE `{primaryKeyName}` = @Id";
            //SELECT* FROM `ProductionShift` WHERE `ProductionShiftID` = @Id

            using var cnn = new MySqlConnection(connectionString);

            var res = await cnn.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });

            return res;
        }

        /// <summary>
        /// Thêm mới bản ghi vào database, trả về số bản ghi bị ảnh hưởng (thường là 1 nếu thành công, 0 nếu thất bại)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Insert(T entity)
        {
            // Lấy tên bảng từ kiểu dữ liệu T
            var tableName = typeof(T).GetTableNameOnly();

            // Lấy tên khóa chính từ kiểu dữ liệu T
            var primaryKeyName = typeof(T).GetPrimaryKey();

            // Lấy danh sách các cột và giá trị tương ứng từ đối tượng entity
            var columns = typeof(T).GetAllColumns();

            // Gán ID mới nếu chưa có, có thể đã tạo ở lúc click nhân bản
            var pkProp = typeof(T).GetProperty(primaryKeyName);
            if (pkProp != null)
            {
                var currentValue = (Guid)(pkProp.GetValue(entity) ?? Guid.Empty);
                if (currentValue == Guid.Empty)
                {
                    pkProp.SetValue(entity, Guid.NewGuid());
                }
            }

            // Build: INSERT INTO `ProductionShift` (`Col1`, `Col2`, ...) VALUES (@Col1, @Col2, ...)
            var columnList = string.Join(", ", columns.Select(c => $"`{c}`"));
            var paramList = string.Join(", ", columns.Select(c => $"@{c}"));
            var sql = $"INSERT INTO `{tableName}` ({columnList}) VALUES ({paramList})";

            // Gán param từ giá trị property
            var param = new DynamicParameters();
            foreach (var col in columns)
            {
                param.Add($"@{col}", entity.GetValueProperty(col));
            }

            using var cnn = new MySqlConnection(connectionString);
            return await cnn.ExecuteAsync(sql, param);
        }      

        public async Task<int> Update(T entity)
        {
            // Lấy tên bảng từ kiểu dữ liệu T
            var tableName = typeof(T).GetTableNameOnly();
            // Lấy tên khóa chính từ kiểu dữ liệu T
            var primaryKeyName = typeof(T).GetPrimaryKey();
            // Lấy danh sách các cột
            var columns = typeof(T).GetAllColumns();

            // Build: UPDATE `ProductionShift` SET `Col1` = @Col1, `Col2` = @Col2, ... WHERE `ProductionShiftCode` = @ProductionShiftCode
            var setClauses = columns
                .Where(c => c != primaryKeyName)
                .Select(c => $"`{c}` = @{c}");

            var sql = $"UPDATE `{tableName}` SET {string.Join(", ", setClauses)} WHERE `{primaryKeyName}` = @{primaryKeyName}";

            var param = new DynamicParameters();

            // lọc và map param
            foreach (var col in columns)
            {
                param.Add($"@{col}", entity.GetValueProperty(col));
            }

            using var cnn = new MySqlConnection(connectionString);
            return await cnn.ExecuteAsync(sql, param);
        }

        /// <summary>
        /// khi thêm mới thì excludeId = null, check toàn bảng
        /// Khi sửa thì truyền ID bản ghi đang sửa vào để không tự trùng với chính nó.
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <param name="excludeId"></param>
        /// <returns></returns>
        public async Task<bool> CheckDuplicate(string propName, object value, Guid? excludeId)
        {
            var tableName = typeof(T).GetTableNameOnly();
            var primaryKey = typeof(T).GetPrimaryKey();

            var sql = $"SELECT COUNT(*) FROM `{tableName}` WHERE `{propName}` = @Value";
            var param = new DynamicParameters();
            param.Add("@Value", value);

            // Khi Update → loại trừ chính bản ghi đang sửa
            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                sql += $" AND `{primaryKey}` <> @ExcludeId";
                param.Add("@ExcludeId", excludeId.Value);
            }

            using var cnn = new MySqlConnection(connectionString);
            var count = await cnn.ExecuteScalarAsync<int>(sql, param);
            // trả về true nếu có bản ghi nào trùng, false nếu không trùng
            return count > 0;
        }




        /// <summary>
        /// Xóa nhiều bản ghi cùng lúc bằng danh sách ID
        /// </summary>
        /// <param name="ids">Danh sách ID của các bản ghi cần xóa</param>
        /// <returns>Số lượng bản ghi bị xóa</returns>
        public async Task<int> Delete(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0) return 0;

            var tableName = typeof(T).GetTableNameOnly();

            // Lấy Id khóa chính từ kiểu dữ liệu T
            var primaryKey = typeof(T).GetPrimaryKey();

            // Build: DELETE FROM `ProductionShift` WHERE `ProductionShiftID` IN (@Id_0, @Id_1, ...)
            var param = new DynamicParameters();
            var paramNames = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                paramNames.Add($"@Id_{i}");
                // Gán param @Id_0, @Id_1,... với giá trị tương ứng từ danh sách ids
                param.Add($"@Id_{i}", ids[i]);
            }

            var sql = $"DELETE FROM `{tableName}` WHERE `{primaryKey}` IN ({string.Join(", ", paramNames)})";

            using var cnn = new MySqlConnection(connectionString);
            return await cnn.ExecuteAsync(sql, param);
        }




        /// <summary>
        /// Tìm kiếm có phân trang kết hợp lọc nhiều điều kiện, sắp xếp
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagingResult<T>> GetFilterPaging(FilterPagingRequest request)
        {
            var tableName = typeof(T).GetTableNameOnly();
            var param = new DynamicParameters();

            // Build WHERE
            //WHERE 1 = 1
            //     AND(`ProductCode` LIKE @Keyword OR `ProductName` LIKE @Keyword)
            //     AND `ProductName` LIKE @Filter_0
            //     AND `Price` >= @Filter_1
            var sqlWhere = SqlFilterBuilder.BuildWhereClause<T>(
                request.Keyword, request.Filters, ref param);

            // Build ORDER BY
            //ORDER BY CreatedDate DESC
            var sqlOrderBy = SqlFilterBuilder.BuildOrderByClause<T>(
                request.SortBy, request.SortDirection);

            // Validate paging
            if (request.PageNumber <= 0) request.PageNumber = 1;
            if (request.PageSize <= 0) request.PageSize = 10;

            var offset = (request.PageNumber - 1) * request.PageSize;
            param.Add("@Offset", offset);
            param.Add("@PageSize", request.PageSize);

            // đếm tổng bản ghi
            var sqlCount = $"SELECT COUNT(*) FROM `{tableName}` {sqlWhere}";

            // lấy data
            var sqlData = $"SELECT * FROM `{tableName}` {sqlWhere} {sqlOrderBy} LIMIT @PageSize OFFSET @Offset";

            using var cnn = new MySqlConnection(connectionString);
            var totalRecord = await cnn.ExecuteScalarAsync<int>(sqlCount, param);
            var data = (await cnn.QueryAsync<T>(sqlData, param)).ToList();

            return new PagingResult<T>
            {
                TotalRecord = totalRecord,
                TotalPage = (int)Math.Ceiling((double)totalRecord / request.PageSize),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Data = data
            };
        }


        // Lấy tất cả data k phân trang để làm Excel

        public async Task<List<T>> GetFilterAll(FilterPagingRequest request)
        {
            var tableName = typeof(T).GetTableNameOnly();
            var param = new DynamicParameters();

            var sqlWhere = SqlFilterBuilder.BuildWhereClause<T>(
                request.Keyword, request.Filters, ref param);
            var sqlOrderBy = SqlFilterBuilder.BuildOrderByClause<T>(
                request.SortBy, request.SortDirection);

            var sql = $"SELECT * FROM `{tableName}` {sqlWhere} {sqlOrderBy}";

            using var cnn = new MySqlConnection(connectionString);
            return (await cnn.QueryAsync<T>(sql, param)).ToList();
        }
    }
}
