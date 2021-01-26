using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Common.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum enumValue)
        {
            var value = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);    //获取描述属性
            if (objs.Length == 0)
            {
                //当描述属性为空，直接返回名称
                return value;
            }
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }

        /// <summary>
        /// 直接获取特性（更轻量、更容易使用，不用封装“获取每一个自定义特性”的扩展方法）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T GetAttributeOfType<T>(this Enum @enum) where T : Attribute
        {
            Type type = @enum.GetType();
            MemberInfo[] memInfo = type.GetMember(@enum.ToString());
            object[] attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }


        public static IEnumerable<(int, T)> GetEnumCustomAttributesList<T>(this Type @enum) where T : Attribute
        {
            if (!@enum.IsEnum)
            {
                return null;
            }
            List<(int, T)> res = new List<(int, T)>();
            foreach (var item in Enum.GetValues(@enum))
            {
                if (item is Enum enumValue)
                {
                    res.Add(((int)item, enumValue.GetAttributeOfType<T>()));

                }
            }
            return res;
        }
    }
}
