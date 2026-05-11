using MISA.PRODUCTION.BL.Base;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Extension;
using MISA.PRODUCTION.Common.Model;
using MISA.PRODUCTION.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.BL.Services
{
    public class ShiftBL : BaseBL<ProductionShift>, IShiftBL
    {
        public ShiftBL(IBaseDL<ProductionShift> baseDL) : base(baseDL)
        {
        }

        /// <summary>
        /// Override Insert để thêm validate riêng cho Shift
        /// </summary>
        public override async Task<int> Insert(ProductionShift entity)
        {
            // Validate nghiệp vụ riêng
            ValidateShift(entity);

            // Tính toán giờ làm việc và giờ nghỉ
            CalculateHours(entity);

            // Gọi base (để check trùng + insert)
            return await base.Insert(entity);
        }

        /// <summary>
        /// Validate các trường bắt buộc + độ dài + giờ nghỉ phải nằm trong giờ làm
        /// </summary>
        private void ValidateShift(ProductionShift entity)
        {
            // Tạo list lỗi để lưu các lỗi phát sinh
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(entity.ProductionShiftCode))
                errors.Add("Mã ca không được để trống");

            if (string.IsNullOrWhiteSpace(entity.ProductionShiftName))
                errors.Add("Tên ca không được để trống");

            if (entity.StartTime == default)
                errors.Add("Giờ vào ca không được để trống");

            if (entity.EndTime == default)
                errors.Add("Giờ hết ca không được để trống");

            if (entity.ProductionShiftCode?.Length > 20)
                errors.Add("Mã ca tối đa 20 ký tự");

            if (entity.ProductionShiftName?.Length > 50)
                errors.Add("Tên ca tối đa 50 ký tự");


            if (entity.BreakStartTime.HasValue && entity.BreakEndTime.HasValue)
            {
                if (entity.BreakStartTime.Value < entity.StartTime)
                    errors.Add("Giờ bắt đầu nghỉ không được trước giờ vào ca");

                if (entity.BreakEndTime.Value > entity.EndTime)
                    errors.Add("Giờ kết thúc nghỉ không được sau giờ hết ca");

                if (entity.BreakStartTime.Value >= entity.BreakEndTime.Value)
                    errors.Add("Giờ bắt đầu nghỉ phải trước giờ kết thúc nghỉ");
            }

            if (errors.Count > 0)
                throw new ValidateException(errors);
        }

        /// <summary>
        /// Tính toán WorkHour và BreakHour từ giờ vào/ra/nghỉ
        /// </summary>
        private void CalculateHours(ProductionShift entity)
        {
            // Tính thời gian nghỉ
            if (entity.BreakStartTime.HasValue && entity.BreakEndTime.HasValue)
            {
                entity.BreakHour = (decimal)(entity.BreakEndTime.Value - entity.BreakStartTime.Value).TotalHours;
            }
            else
            {
                entity.BreakHour = 0;
            }

            // Tính thời gian làm việc = (EndTime - StartTime) - BreakHour
            var totalHours = (decimal)(entity.EndTime - entity.StartTime).TotalHours;
            entity.WorkHour = totalHours - entity.BreakHour;
        }
    }
}
