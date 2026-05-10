using MISA.PRODUCTION.BL.Base;
using MISA.PRODUCTION.BL.Interfaces;
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
    }
}
