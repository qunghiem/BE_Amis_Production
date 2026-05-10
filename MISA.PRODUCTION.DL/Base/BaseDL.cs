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

    }
}
