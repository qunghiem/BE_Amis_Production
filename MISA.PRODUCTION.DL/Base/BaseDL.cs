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
        private string connectionString = "server=localhost;port=3306;database=misa_amisproduction;user=root;password=root;";

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

            var sql = $"SELECT * FROM {tableName} WHERE {primaryKeyName} = @Id";
            //SELECT* FROM `ProductionShift` WHERE `ProductionShiftID` = @Id

            using var cnn = new MySqlConnection(connectionString);

            var res = await cnn.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });

            return res;
        } 

        public async Task<int> Insert(T entity)
        {
            // Lấy tên bảng từ kiểu dữ liệu T
            var tableName = typeof(T).GetTableNameOnly();

            // Lấy tên khóa chính từ kiểu dữ liệu T
            var primaryKeyName = typeof(T).GetPrimaryKey();

            // Lấy danh sách các cột và giá trị tương ứng từ đối tượng entity
            var columns = typeof(T).GetAllColumns();

            // Gán ID mới nếu chưa có
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
    }
}
