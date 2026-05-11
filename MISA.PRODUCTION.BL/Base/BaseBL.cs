using MISA.PRODUCTION.BL.Interfaces;
using MISA.PRODUCTION.Common.Attributes;
using MISA.PRODUCTION.Common.Extension;
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

        // Lấy bản ghi theo ID
        public async Task<T> GetById(Guid id)
        {
            return await _baseDL.GetById(id);
        }

        // Hàm thêm mới bản ghi
        // virtual để các lớp con có thể override nếu cần
        public virtual async Task<int> Insert(T entity)
        {
            await ValidateCheckDuplicate(entity, isInsert: true); 
            return await _baseDL.Insert(entity);
        }

        /// <summary>
        /// Check trùng các property có gắn [CheckDuplicate]
        /// </summary>
        protected async Task ValidateCheckDuplicate(T entity, bool isInsert)
        {
            //
            var errors = new List<string>();

            // Lấy ID để loại trừ khi update
            Guid? excludeId = null;
            if (!isInsert)
            {
                // Lấy giá trị ID của entity để loại trừ khi kiểm tra trùng lặp
                var pkName = typeof(T).GetPrimaryKey();
                // Sử dụng reflection để lấy giá trị ID từ entity
                var pkProp = typeof(T).GetProperty(pkName);
                if (pkProp != null)
                {
                    // Lấy giá trị ID và gán vào excludeId
                    excludeId = (Guid?)pkProp.GetValue(entity);
                }
            }

            // Duyệt property có gắn [CheckDuplicate]
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                // Kiểm tra nếu property có gắn [CheckDuplicate]
                var attr = (CheckDuplicateAttribute?)Attribute.GetCustomAttribute(prop, typeof(CheckDuplicateAttribute));
                if (attr == null) continue;

                // Lấy giá trị của property
                var value = prop.GetValue(entity);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString())) continue;

                // Gọi hàm kiểm tra trùng lặp trong DL, truyền tên property, giá trị và ID để loại trừ
                var isDuplicate = await _baseDL.CheckDuplicate(prop.Name, value, excludeId);
                // Nếu trùng lặp, thêm lỗi vào danh sách
                if (isDuplicate)
                {
                    errors.Add(attr.ErrorMessage);
                }
            }

            // Nếu có lỗi, ném ValidateException với danh sách lỗi
            if (errors.Count > 0)
            {
                throw new ValidateException(errors);
            }
        }
    }
}
