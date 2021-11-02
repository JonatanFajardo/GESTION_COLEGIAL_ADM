using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Extensions
{
    public static class ObjectExtension
    {
        public static object GetValuePropertie<T>(object obj, string name)
        {
            Type temp = typeof(T);
            //IfNotExist
            foreach (var item in obj.GetType().GetProperties())
            {
                
            }
            var property = obj.GetType().GetProperty(name);
            var value = property.GetValue(obj, null);
            List<dynamic> list = new List<dynamic>();
            return value;
            //foreach (PropertyInfo property in temp.GetProperties())
            //{
            //    if (property.Name == name)
            //    {
            //        list.Add(property.GetValue);
            //    }
            //}
        }
    }
}
