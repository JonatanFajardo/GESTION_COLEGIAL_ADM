using System;
using System.Collections.Generic;

namespace GESTION_COLEGIAL.Business.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Compara una propiedad específica de dos objetos.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Objeto a comparar</param>
        /// <param name="nameProperty">Nombre de propiedad</param>
        /// <param name="obj2">Objeto de comparación</param>
        /// <param name="nameProperty2">Nombre de propiedad</param>
        /// <param name="NameReturn"></param>
        /// <returns></returns>
        public static object EqualCheckBoxProperties<T>(dynamic obj, string nameProperty, dynamic obj2, string nameProperty2, string NameReturn)
        {
            Type temp = typeof(T);
            var newObject = Activator.CreateInstance(temp);
            List<bool> result = new List<bool>();
            List<dynamic> list1 = GetValueProperty(obj, nameProperty); //Datos seleccionados en la base de datos
            List<dynamic> list2 = GetValueProperty(obj2, nameProperty2); //Datos de la SelectList

            bool status = false;
            for (int i = 0; i < list2.Count; i++)
            {
                status = false;
                for (int j = 0; j < list1.Count; j++)
                {
                    if (Convert.ToInt32(list2[i]) == Convert.ToInt32(list1[j]))
                    {
                        status = true;
                    }
                }
                result.Add(status);
            }
            SetValueToProperty<T>(obj2, NameReturn, result);
            return result;
        }

        /// <summary>
        /// Modifica los valores de toda la columna.
        /// </summary>
        /// <param name="obj">Contiene una lista de objetos</param>
        /// <param name="name">Nombre de la propiedad</param>
        /// <param name="newValues">Listado con los valores a cambiar en la columna.</param>
        public static object SetValueToProperty<T>(dynamic obj, string name, dynamic newValues)
        {
            Type temp = typeof(T);
            var newObject = Activator.CreateInstance<T>();
            if (obj != null && newValues != null)
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    var property2 = obj[i].GetType().GetProperty(name);
                    property2.SetValue(obj[i], newValues[i], null);
                }
            }
            return newObject;
        }

        /// <summary>
        /// Obtiene un listado con los valores de la propiedad especificada del objeto.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<dynamic> GetValueProperty(dynamic obj, string name)
        {
            List<dynamic> listValues = new List<dynamic>();
            //IfNotExist
            if (obj != null)
            {
                foreach (var item in obj)
                {
                    var property2 = item.GetType().GetProperty(name);
                    var value2 = property2.GetValue(item, null);
                    listValues.Add(value2);
                }
            }

            return listValues;
        }
    }
}