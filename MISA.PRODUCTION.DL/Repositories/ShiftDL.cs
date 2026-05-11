using Dapper;
using MISA.PRODUCTION.Common.Model;
using MISA.PRODUCTION.DL.Base;
using MISA.PRODUCTION.DL.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.DL.Repositories
{

    public class ShiftDL : BaseDL<ProductionShift>, IShiftDL
    {
        /// <summary>
        /// Hàm thay đổi trạng thái Sử dụng -> Ngưng sử dụng
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<int> ToggleStatus(List<Guid> ids, int status)
        {
            if (ids == null || ids.Count == 0) return 0;

            var param = new DynamicParameters();

            var paramNames = new List<string>();

            for (int i = 0; i < ids.Count; i++)
            {
                paramNames.Add($"@Id_{i}");
                param.Add($"@Id_{i}", ids[i]);
            }
            param.Add("@Status", status);
            param.Add("@ModifiedDate", DateTime.Now);

            var sql = $@"UPDATE `ProductionShift` 
                         SET `ShiftStatus` = @Status, 
                             `ModifiedDate` = @ModifiedDate                    
                         WHERE `ProductionShiftID` IN ({string.Join(", ", paramNames)})";

            using var cnn = new MySqlConnection(connectionString);
            return await cnn.ExecuteAsync(sql, param);
        }
    }
}
