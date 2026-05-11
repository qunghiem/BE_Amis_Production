using MISA.PRODUCTION.BL.Base;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Extension;
using MISA.PRODUCTION.Common.Model;
using MISA.PRODUCTION.DL.Interfaces;
using MISA.PRODUCTION.DL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.BL.Services
{
    public class ShiftBL : BaseBL<ProductionShift>, IShiftBL
    {
        private IShiftDL _shiftDL;
        //public ShiftBL(IBaseDL<ProductionShift> baseDL) : base(baseDL)
        //{
        //}

        public ShiftBL(IShiftDL shiftDL) : base(shiftDL)
        {
            _shiftDL = shiftDL;
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
        /// Override Update để thêm validate riêng cho Shift
        /// </summary>

        public override async Task<int> Update(ProductionShift entity)
        {
            // Validate nghiệp vụ riêng
            ValidateShift(entity);

            // Tính toán giờ làm việc và giờ nghỉ

            CalculateHours(entity);

            // Gọi base (để check trùng + Update)
            return await base.Update(entity);
        }

        /// <summary>
        /// Validate các trường bắt buộc + độ dài + giờ nghỉ phải nằm trong giờ làm
        /// </summary>
        private void ValidateShift(ProductionShift entity)
        {
            // Tạo list lỗi để lưu các lỗi phát sinh
            var errors = new List<string>();

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(entity.ProductionShiftCode))
                errors.Add("Mã ca không được để trống");

            if (string.IsNullOrWhiteSpace(entity.ProductionShiftName))
                errors.Add("Tên ca không được để trống");

            if (entity.StartTime == default)
                errors.Add("Giờ vào ca không được để trống");

            if (entity.EndTime == default)
                errors.Add("Giờ hết ca không được để trống");

            // Validate độ dài
            if (entity.ProductionShiftCode?.Length > 20)
                errors.Add("Mã ca tối đa 20 ký tự");

            if (entity.ProductionShiftName?.Length > 50)
                errors.Add("Tên ca tối đa 50 ký tự");


            // Validate giờ nghỉ phải nằm trong khoảng giờ làm
            if (entity.BreakStartTime.HasValue && entity.BreakEndTime.HasValue)
            {
                // Chỉ validate khi ca KHÔNG xuyên ngày
                if (entity.StartTime < entity.EndTime)
                {
                    if (entity.BreakStartTime.Value < entity.StartTime)
                        errors.Add("Giờ bắt đầu nghỉ không được trước giờ vào ca");

                    if (entity.BreakEndTime.Value > entity.EndTime)
                        errors.Add("Giờ kết thúc nghỉ không được sau giờ hết ca");
                }

                // Luôn check: giờ bắt đầu nghỉ phải trước giờ kết thúc nghỉ
                if (entity.BreakStartTime.Value >= entity.BreakEndTime.Value)
                    errors.Add("Giờ bắt đầu nghỉ phải trước giờ kết thúc nghỉ");
            }

            // Nếu có lỗi, ném ra ValidateException với danh sách lỗi
            if (errors.Count > 0)
                throw new ValidateException(errors);
        }

        /// <summary>
        /// Tính toán WorkHour và BreakHour từ giờ vào/ra/nghỉ
        /// </summary>
        private void CalculateHours(ProductionShift entity)
        {
            // Tính tổng giờ ca, nếu EndTime < StartTime → ca xuyên ngày → cộng 24h
            var totalHours = (decimal)(entity.EndTime - entity.StartTime).TotalHours;
            if (entity.EndTime <= entity.StartTime)
            {
                totalHours += 24;
            }

            // Tính giờ nghỉ, cũng +24 nếu nghỉ xuyên ngày
            if (entity.BreakStartTime.HasValue && entity.BreakEndTime.HasValue)
            {
                var breakHours = (decimal)(entity.BreakEndTime.Value - entity.BreakStartTime.Value).TotalHours;
                if (entity.BreakEndTime.Value <= entity.BreakStartTime.Value)
                {
                    breakHours += 24;
                }
                entity.BreakHour = breakHours;
            }
            else
            {
                entity.BreakHour = 0;
            }

            // Giờ làm thực = tổng giờ ca - giờ nghỉ
            entity.WorkHour = totalHours - entity.BreakHour;
        }

        /// <summary>
        /// Hàm nhân bản ca làm việc: copy toàn bộ thông tin từ ca gốc, chỉ thay ID mới + Mã ca để trống
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ValidateException"></exception>
        public async Task<ProductionShift> DuplicateShift(Guid id)
        {
            // Lấy bản ghi gốc
            var source = await _baseDL.GetById(id);

            if (source == null)
                throw new ValidateException("Không tìm thấy ca làm việc cần nhân bản");

            // Copy toàn bộ trừ ID và Mã ca
            var newShift = new ProductionShift
            {
                //ProductionShiftID = Guid.NewGuid(),
                ProductionShiftCode = "",               // để Mã ca trống, FE cho user nhập
                ProductionShiftName = source.ProductionShiftName,
                StartTime = source.StartTime,
                EndTime = source.EndTime,
                BreakStartTime = source.BreakStartTime,
                BreakEndTime = source.BreakEndTime,
                WorkHour = source.WorkHour,
                BreakHour = source.BreakHour,
                Description = source.Description,
                ShiftStatus = source.ShiftStatus
            };

            return newShift;    // Chỉ trả về data, KHÔNG lưu DB
        }


        public async Task<int> ToggleStatus(List<Guid> ids, int status)
        {
            if (ids == null || ids.Count == 0)
                throw new ValidateException("Không có dữ liệu để cập nhật");

            if (status != 0 && status != 1)
                throw new ValidateException("Trạng thái không hợp lệ");

            return await _shiftDL.ToggleStatus(ids, status);
        }
    }
}
