using MISA.PRODUCTION.Common.Attributes;
using MISA.PRODUCTION.Common.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Model
{

    [ConfigTable("ProductionShift")] // Tên bảng trong database
    public class ProductionShift : BaseEntity
    {
        /// <summary>
        /// ID của ca sản xuất, là khóa chính, kiểu dữ liệu GUID để đảm bảo tính duy nhất trên toàn hệ thống
        /// </summary>
        [Key]
        public Guid ProductionShiftID { get; set; }

        /// <summary>
        /// Mã ca sản xuất
        /// có độ dài tối đa 20 ký tự, và phải là duy nhất (không được trùng với mã ca khác)
        /// </summary>
        [CheckDuplicate("Mã ca đã bị trùng")]        
        public string ProductionShiftCode { get; set; }

        /// <summary>
        /// Tên ca sản xuất
        /// có độ dài tối đa 50 ký tự
        /// </summary>
        public string ProductionShiftName { get; set; }

        /// <summary>
        /// Giờ bắt đầu ca
        /// chỉ lưu thời gian, k lưu ngày
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Giờ kết thúc ca
        /// chỉ lưu thời gian, k lưu ngày
        /// </summary>
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Giờ ban đầu ca nghỉ giữa ca
        /// </summary>
        public TimeSpan? BreakStartTime { get; set; }

        /// <summary>
        /// Giờ kết thúc ca nghỉ giữa ca
        /// </summary>
        public TimeSpan? BreakEndTime { get; set; }

        /// <summary>
        /// Thời gian làm việc thực tế của ca, đã trừ đi thời gian nghỉ giữa ca
        /// </summary>
        public decimal WorkHour { get; set; }

        /// <summary>
        /// Thời gian nghỉ giữa ca
        /// </summary>
        public decimal BreakHour { get; set; }

        /// <summary>
        /// Mô tả thêm về ca
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái ca (1: Hoạt động, 0: Ngừng hoạt động)
        /// </summary>
        public int ShiftStatus { get; set; } = 1;
    }
}
