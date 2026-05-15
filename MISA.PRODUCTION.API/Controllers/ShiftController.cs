using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Extension;
using MISA.PRODUCTION.Common.Model;
using MISA.PRODUCTION.Common.Resources;

namespace MISA.PRODUCTION.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ShiftController : BaseController<ProductionShift>
    {
        private IShiftBL _shiftBL;

        public ShiftController(IShiftBL shiftBL) : base(shiftBL)
        {
            _shiftBL = shiftBL;
        }

        #region API nhân bản ca làm việc
        /// <summary>
        /// API nhân bản ca làm việc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("duplicate/{id}")]
        public async Task<IActionResult> Duplicate(Guid id)
        {
            try
            {
                var res = await _shiftBL.DuplicateShift(id);
                return Ok(res);
            }
            catch (ValidateException ex)
            {
                return BadRequest(new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ex.Message,
                    MoreInfo = ex.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ResourceVN.Exception,
                    MoreInfo = ex.Data
                });
            }
        }
        #endregion


        #region API chuyển đổi trạng thái cho ca làm việc: sử dụng -> ngưng sử dụng
        /// <summary>
        /// API chuyển đổi trạng thái cho ca làm việc: sử dụng -> ngưng sử dụng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequest request)
        {
            try
            {
                var res = await _shiftBL.ToggleStatus(request.Ids, request.Status);
                return Ok(res);
            }
            catch (ValidateException ex)
            {
                return BadRequest(new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ex.Message,
                    MoreInfo = ex.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ResourceVN.Exception,
                    MoreInfo = ex.Data
                });
            }
        }
        #endregion


        /// <summary>
        /// Xuất file Excel danh sách ca làm việc theo điều kiện lọc, phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportExcel([FromBody] FilterPagingRequest request)
        {
            try
            {
                var fileBytes = await _shiftBL.ExportExcel(request);
                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"CaLamViec_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ResourceVN.Exception,
                    MoreInfo = ex.Data
                });
            }
        }
    }
}
