using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Extension;
using MISA.PRODUCTION.Common.Model;
using MISA.PRODUCTION.Common.Resources;

namespace MISA.PRODUCTION.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        protected IBaseBL<T> _baseBL;

        public BaseController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }

        #region Lấy bản ghi theo ID
        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var res = await _baseBL.GetById(id);

                if (res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound(new ErrorResult
                    {
                        DevMsg = "Resource not found with the provided ID",
                        UserMsg = ResourceVN.NotFound,
                        MoreInfo = id
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ResourceVN.Exception,
                    MoreInfo = ex.Data,
                });
            }
        }
        #endregion

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] T entity)
        {
            try
            {
                var res = await _baseBL.Insert(entity);
                return StatusCode(201, res);
            }
            // bắt lỗi người dùng: dữ liệu không hợp lệ, thiếu trường bắt buộc, ...
            catch (ValidateException ex)
            {
                return BadRequest(new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ex.Message,
                    MoreInfo = ex.Errors
                });
            }
            // bắt lỗi server: lỗi kết nối database, lỗi code, ...
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResult
                {
                    DevMsg = ex.Message,
                    UserMsg = ResourceVN.Exception,
                    MoreInfo = ex.Data,
                });
            }
        }
    }
}
