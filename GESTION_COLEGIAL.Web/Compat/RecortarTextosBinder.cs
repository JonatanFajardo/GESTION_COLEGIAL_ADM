using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Equivalente al binder global de MVC 5 (BaseViewModel : DefaultModelBinder):
    /// en las propiedades de texto de los ViewModels recorta espacios y deja null si quedan vacías.
    /// Como en MVC 5, no toca los parámetros sueltos de las acciones, solo propiedades.
    /// </summary>
    public class RecortarTextosBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(string)
                || context.Metadata.MetadataKind != ModelMetadataKind.Property)
            {
                return null;
            }

            // Solo datos de formulario, query string o ruta (no headers, body, etc.).
            var origen = context.BindingInfo?.BindingSource;
            if (origen != null
                && origen != BindingSource.Form
                && origen != BindingSource.Query
                && origen != BindingSource.Path
                && origen != BindingSource.ModelBinding)
            {
                return null;
            }

            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new RecortarTextosBinder(new SimpleTypeModelBinder(typeof(string), loggerFactory));
        }
    }

    public class RecortarTextosBinder : IModelBinder
    {
        private readonly IModelBinder _binderTexto;

        public RecortarTextosBinder(IModelBinder binderTexto)
        {
            _binderTexto = binderTexto;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _binderTexto.BindModelAsync(bindingContext);

            if (bindingContext.Result.IsModelSet && bindingContext.Result.Model is string texto)
            {
                var limpio = string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
                bindingContext.Result = ModelBindingResult.Success(limpio);
            }
        }
    }
}
