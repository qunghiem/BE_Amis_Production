using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Model;

namespace MISA.PRODUCTION.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ShiftController : BaseController<ProductionShift>
    {
        public ShiftController(IShiftBL shiftBL) : base(shiftBL)
        {
        }
    }
}
