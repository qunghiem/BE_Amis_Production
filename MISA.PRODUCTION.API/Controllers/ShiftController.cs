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

        /// <summary>
        /// Hàm nhân bản ca làm việc
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
    }
}
