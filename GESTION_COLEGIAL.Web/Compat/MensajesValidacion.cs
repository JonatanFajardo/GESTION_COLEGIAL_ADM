using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Mensajes de validación por defecto en español.
    /// MVC 5 los traía por el paquete de idioma (Microsoft.AspNet.Mvc.es); ASP.NET Core solo los trae en inglés.
    /// Solo se aplican a las anotaciones que NO tienen ErrorMessage propio.
    /// </summary>
    public static class MensajesValidacion
    {
        public static void Traducir(ValidationAttribute atributo)
        {
            if (!string.IsNullOrEmpty(atributo.ErrorMessage)
                || !string.IsNullOrEmpty(atributo.ErrorMessageResourceName)
                || atributo.ErrorMessageResourceType != null)
            {
                return;
            }

            var plantilla = PlantillaPara(atributo);
            if (plantilla != null)
            {
                atributo.ErrorMessage = plantilla;
            }
        }

        public static void ConfigurarModelBinding(DefaultModelBindingMessageProvider mensajes)
        {
            mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
            mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El campo debe ser un número.");
            mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
            mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
            mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor indicado no es válido para {campo}.");
            mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor indicado no es válido.");
            mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
            mensajes.SetValueMustNotBeNullAccessor(valor => $"El valor '{valor}' no es válido.");
            mensajes.SetMissingBindRequiredValueAccessor(campo => $"No se indicó un valor para '{campo}'.");
            mensajes.SetMissingKeyOrValueAccessor(() => "Se requiere un valor.");
            mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "Se requiere el contenido de la solicitud.");
        }

        private static string PlantillaPara(ValidationAttribute atributo) => atributo switch
        {
            RequiredAttribute => "El campo {0} es obligatorio.",
            StringLengthAttribute largo when largo.MinimumLength > 0 =>
                "El campo {0} debe ser una cadena con una longitud mínima de {2} y una longitud máxima de {1}.",
            StringLengthAttribute => "El campo {0} debe ser una cadena con una longitud máxima de {1}.",
            RangeAttribute => "El campo {0} debe estar entre {1} y {2}.",
            RegularExpressionAttribute => "El campo {0} debe coincidir con la expresión regular '{1}'.",
            MaxLengthAttribute => "El campo {0} debe ser una cadena o una lista con una longitud máxima de '{1}'.",
            MinLengthAttribute => "El campo {0} debe ser una cadena o una lista con una longitud mínima de '{1}'.",
            EmailAddressAttribute => "El campo {0} no es una dirección de correo electrónico válida.",
            PhoneAttribute => "El campo {0} no es un número de teléfono válido.",
            UrlAttribute => "El campo {0} no es una dirección web válida (http, https o ftp).",
            CreditCardAttribute => "El campo {0} no es un número de tarjeta de crédito válido.",
            System.ComponentModel.DataAnnotations.CompareAttribute => "'{0}' y '{1}' no coinciden.",
            RemoteAttributeBase => "'{0}' no es válido.",
            _ => null
        };
    }

    /// <summary>
    /// Traduce las anotaciones de los ViewModels (validación en servidor y en el navegador).
    /// </summary>
    public class MensajesValidacionMetadataProvider : IValidationMetadataProvider
    {
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            foreach (var atributo in context.ValidationMetadata.ValidatorMetadata.OfType<ValidationAttribute>())
            {
                MensajesValidacion.Traducir(atributo);
            }
        }
    }

    /// <summary>
    /// MVC 5 marcaba los campos enteros con data-val-number ("El campo X debe ser un número.");
    /// ASP.NET Core solo lo hace con decimales. Esto lo repone para int, long, short y byte.
    /// </summary>
    public class NumerosEnterosClientValidatorProvider : Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IClientModelValidatorProvider
    {
        private static readonly HashSet<Type> TiposEnteros = new HashSet<Type>
        {
            typeof(int), typeof(long), typeof(short), typeof(byte),
            typeof(uint), typeof(ulong), typeof(ushort), typeof(sbyte)
        };

        public void CreateValidators(Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientValidatorProviderContext context)
        {
            var tipo = Nullable.GetUnderlyingType(context.ModelMetadata.ModelType) ?? context.ModelMetadata.ModelType;
            if (!TiposEnteros.Contains(tipo) || context.Results.Any(r => r.Validator is NumeroEnteroClientValidator))
            {
                return;
            }

            context.Results.Add(new Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientValidatorItem
            {
                Validator = new NumeroEnteroClientValidator(),
                IsReusable = true
            });
        }
    }

    public class NumeroEnteroClientValidator : Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IClientModelValidator
    {
        public void AddValidation(Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ClientModelValidationContext context)
        {
            if (!context.Attributes.ContainsKey("data-val"))
            {
                context.Attributes["data-val"] = "true";
            }

            if (!context.Attributes.ContainsKey("data-val-number"))
            {
                context.Attributes["data-val-number"] = $"El campo {context.ModelMetadata.GetDisplayName()} debe ser un número.";
            }
        }
    }

    /// <summary>
    /// Cubre también el [Required] implícito que ASP.NET Core agrega a los números (ej. Car_Id)
    /// al generar los atributos data-val-* del navegador.
    /// </summary>
    public class MensajesValidacionAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _proveedorBase = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            MensajesValidacion.Traducir(attribute);
            return _proveedorBase.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
