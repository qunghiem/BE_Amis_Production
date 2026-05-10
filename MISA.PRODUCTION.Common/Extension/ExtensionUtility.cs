using MISA.PRODUCTION.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.PRODUCTION.Common.Extension
{
    /// <summary>
    /// thư viện extension để lấy ra thông tin của model như tên bảng, tên cột, kiểu dữ liệu của cột phục vụ cho việc xây dựng câu query động trong BL
    /// </summary>
    public static class ExtensionUtility
    {
        /// <summary>
        /// Lấy ra tên bảng của model
        /// nếu có gắn attribute ConfigTable thì lấy tên bảng trong attribute
        /// nếu không có thì lấy tên class làm tên bảng
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableNameOnly(this Type type)
        {
            var attr = (ConfigTableAttribute)type
                .GetCustomAttributes(typeof(ConfigTableAttribute), true)
                ?.FirstOrDefault();
            return attr?.TableName ?? type.Name;
        }

        /// <summary>
        /// tìm thuộc tính có gắn nhãn [Key] và trả về Tên của thuộc tính đó.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPrimaryKey(this Type type)
        {
            var prop = type.GetProperties()
                .SingleOrDefault(p => p.GetCustomAttribute(typeof(KeyAttribute), true) != null); // !=null tức là có attribute Key, nghĩa là thuộc tính này được đánh dấu là khóa chính
            return prop?.Name ?? string.Empty;
        }

        /// <summary>
        /// Lấy thuộc tính k có gắn nhãn [NotMapped] và trả về Tên của thuộc tính đó.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetAllColumns(this Type type)
        {
            return type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), true) == null) // ==null tức là không có attribute NotMapped, nghĩa là thuộc tính này sẽ được map vào cột trong database
                .Select(p => p.Name)
                .ToList();
        }

        /// <summary>
        /// Lấy thuộc tính có kiểu dữ liệu là string và không có gắn nhãn [NotMapped] và trả về Tên của thuộc tính đó.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetStringColumns(this Type type)
        {
            return type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), true) == null
                         && p.PropertyType == typeof(string))
                .Select(p => p.Name)
                .ToList();
        }

        /// <summary>
        /// lấy giá trị của thuộc tính theo tên thuộc tính của 1 đối tượng
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValueProperty(this object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName)) return null;
            // tìm và lấy thông tin thuộc tính theo tên
            var info = obj.GetType().GetProperty(propertyName);
            return info?.GetValue(obj);
        }

    }
}
