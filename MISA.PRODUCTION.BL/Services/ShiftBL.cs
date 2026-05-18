using ClosedXML.Excel;
using MISA.PRODUCTION.BL.Base;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Enums;
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

            //if (entity.StartTime == default)
            //    errors.Add("Giờ vào ca không được để trống");

            //if (entity.EndTime == default)
            //    errors.Add("Giờ hết ca không được để trống");

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
                        errors.Add("Thời gian bắt đầu nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca. Vui lòng kiểm tra lại.");

                    if (entity.BreakEndTime.Value > entity.EndTime)
                        errors.Add("Thời gian kết thúc nghỉ giữa ca phải nằm trong khoảng thời gian tính từ giờ vào ca đến giờ hết ca. Vui lòng kiểm tra lại.");
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
                entity.BreakHour = Math.Ceiling(breakHours); ;
            }
            else
            {
                entity.BreakHour = 0;
            }

            // Giờ làm thực = tổng giờ ca - giờ nghỉ
            entity.WorkHour = Math.Ceiling(totalHours - entity.BreakHour);

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

        // Hàm chuyển đổi trạng thái ca
        public async Task<int> ToggleStatus(List<Guid> ids, ShiftStatus status)
        {
            if (ids == null || ids.Count == 0)
                throw new ValidateException("Không có dữ liệu để cập nhật");

            if (!Enum.IsDefined(status))
                throw new ValidateException("Trạng thái không hợp lệ");

            return await _shiftDL.ToggleStatus(ids, status);
        }

        // Hàm Tạo Excel
        public async Task<byte[]> ExportExcel(FilterPagingRequest request)
        {
            // get toàn bộ ds k phân trang
            var data = await _baseDL.GetFilterAll(request);
            // Tạo ra một file Excel trống
            using var workbook = new XLWorkbook();
            //Tạo một sheet mới tên là "Ca làm việc"
            var ws = workbook.Worksheets.Add("Ca làm việc");

            // Tạo hàng Tiêu đề Header — các cột
            var headers = new[]
            {
                "STT", "Mã ca", "Tên ca", "Giờ vào ca", "Giờ hết ca",
                "Bắt đầu nghỉ giữa ca", "Kết thúc nghỉ giữa ca",
                "Thời gian làm việc (giờ)", "Thời gian nghỉ giữa ca (giờ)",
                "Trạng thái", "Người tạo", "Ngày tạo", "Người sửa", "Ngày sửa"
            };

            // style từng ô cho hàng
            // // 1 ô(cell) dc tạo bởi (hàng, cột)
            for (int i = 0; i < headers.Length; i++)
            {
                // lấy ra ô đó
                var cell = ws.Cell(1, i + 1);
                // gán giá trị cho ô 
                cell.Value = headers[i];
                // set style cho ô
                cell.Style.Font.Bold = true;
            }

            // Data
            // lặp qua từng bản ghi 
            for (int row = 0; row < data.Count; row++)
            {
                // lấy ra bản ghi
                var s = data[row];
                // tính stt hàng để chứa row, vì header ở hàng 1 nên row bắt đầu từ 2
                var r = row + 2;

                // ô STT
                ws.Cell(r, 1).Value = row + 1;
                // ô Mã ca
                ws.Cell(r, 2).Value = s.ProductionShiftCode;
                // ô Tên ca
                ws.Cell(r, 3).Value = s.ProductionShiftName;
                // ô thơi gian bắt đầu ca
                ws.Cell(r, 4).Value = s.StartTime.ToString(@"hh\:mm");
                // ô thời gian kết thúc ca
                ws.Cell(r, 5).Value = s.EndTime.ToString(@"hh\:mm");
                ws.Cell(r, 6).Value = s.BreakStartTime?.ToString(@"hh\:mm") ?? "";
                ws.Cell(r, 7).Value = s.BreakEndTime?.ToString(@"hh\:mm") ?? "";
                ws.Cell(r, 8).Value = s.WorkHour;
                ws.Cell(r, 9).Value = s.BreakHour;
                ws.Cell(r, 10).Value = s.ShiftStatus == ShiftStatus.Active? "Đang sử dụng" : "Ngừng sử dụng";
                ws.Cell(r, 11).Value = s.CreatedBy ?? "";
                ws.Cell(r, 12).Value = s.CreatedDate.ToString("dd/MM/yyyy");
                ws.Cell(r, 13).Value = s.ModifiedBy ?? "";
                ws.Cell(r, 14).Value = s.ModifiedDate.ToString("dd/MM/yyyy");
            }
            // chọn toàn bộ cột đang có dữ liệu và tự động co giãn width dựa theo nội dung
            ws.Columns().AdjustToContents();

            // new 1 vùng nhớ trong RAM
            using var stream = new MemoryStream();
            // đổ toàn bộ dữ liệu file Excel đó vào vùng đệm RAM
            workbook.SaveAs(stream);
            // trả về kiểu dữ liệu mảng các byte thô
            return stream.ToArray();
        }
    }
}
