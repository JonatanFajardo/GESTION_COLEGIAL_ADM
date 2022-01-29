using System.ComponentModel;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
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
                    var ss = value;
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
}