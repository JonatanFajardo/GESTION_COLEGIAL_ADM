using System.ComponentModel;

namespace GESTION_COLEGIAL.Business.Models
{
#if NETFRAMEWORK
    /// <summary>
    /// Clase base de los ViewModels. En MVC 5 además es el binder global
    /// (registrado en Global.asax): recorta espacios en los textos y convierte los vacíos en null.
    /// </summary>
    public class BaseViewModel : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext,
                                            ModelBindingContext bindingContext,
                                            PropertyDescriptor propertyDescriptor,
                                            object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = (string)value;
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    value = stringValue.Trim();
                }
                else
                {
                    value = null;
                }
            }

            base.SetProperty(controllerContext,
                                bindingContext,
                                propertyDescriptor,
                                value);
        }
    }
#else
    /// <summary>
    /// Clase base de los ViewModels. En ASP.NET Core el recorte de espacios
    /// lo hace un model binder propio que se registra en la UI nueva.
    /// </summary>
    public class BaseViewModel
    {
    }
#endif
}
