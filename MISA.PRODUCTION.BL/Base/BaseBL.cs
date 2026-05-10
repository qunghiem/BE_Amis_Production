using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.BL.Base
{
    public class BaseBL<T> : IBaseBL<T>
    {
        protected IBaseDL<T> _baseDL;

        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }

        public async Task<T> GetById(Guid id)
        {
            return await _baseDL.GetById(id);
        }
    }
}
