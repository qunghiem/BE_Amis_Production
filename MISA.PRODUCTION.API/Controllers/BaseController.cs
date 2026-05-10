using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PRODUCTION.BL.Interfaces;
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
    }
}
