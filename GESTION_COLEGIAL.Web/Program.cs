using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.WebEncoders;
using GESTION_COLEGIAL.UI.Compat;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;

// Arranque de la UI. Reemplaza Global.asax, App_Start (rutas, filtros, bundles) y Web.config.

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews(opciones =>
    {
        // .NET Core quita el sufijo "Async" de las acciones (ListAsync → List) y las URLs
        // del JS (/alumnos/listasync) darían 404. Se conservan los nombres como en MVC 5.
        opciones.SuppressAsyncSuffixInActionNames = false;

        // Igual que el binder global de MVC 5: recorta espacios en los textos y los vacíos quedan en null.
        opciones.ModelBinderProviders.Insert(0, new RecortarTextosBinderProvider());

        // Mensajes de validación en español, como los traía MVC 5.
        opciones.ModelMetadataDetailsProviders.Add(new MensajesValidacionMetadataProvider());
        MensajesValidacion.ConfigurarModelBinding(opciones.ModelBindingMessageProvider);
    })
    .AddNewtonsoftJson(opciones =>
    {
        // Las vistas leen las propiedades tal cual vienen (Alu_Id), sin pasarlas a camelCase.
        opciones.SerializerSettings.ContractResolver = new DefaultContractResolver();
    })
    .AddViewOptions(opciones =>
    {
        // Como en MVC 5: el hidden de cada CheckBoxFor va junto al checkbox, no al final del form.
        opciones.HtmlHelperOptions.CheckBoxHiddenInputRenderMode = CheckBoxHiddenInputRenderMode.Inline;

        // Como en MVC 5: los campos enteros llevan data-val-number.
        opciones.ClientModelValidatorProviders.Add(new NumerosEnterosClientValidatorProvider());
    });

// Escribir acentos y ñ tal cual en el HTML ("Año", no "A&#xF1;o"); también evita entidades
// dentro de los textos que las vistas meten en JavaScript.
builder.Services.Configure<WebEncoderOptions>(opciones =>
    opciones.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

builder.Services.AddSingleton<IValidationAttributeAdapterProvider, MensajesValidacionAdapterProvider>();
builder.Services.AddRouting(opciones => opciones.LowercaseUrls = true);

// Sesión: solo guarda datos que cambian durante la visita (ej. "ver como rol" de DevTools).
// Los datos del usuario logueado viajan en la cookie de login (ver Compat/SesionUsuario.cs).
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opciones =>
{
    opciones.IdleTimeout = TimeSpan.FromMinutes(60);
    opciones.Cookie.HttpOnly = true;
    opciones.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/account/login";
        opciones.AccessDeniedPath = "/account/sinacceso";
        opciones.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        opciones.SlidingExpiration = true;
    });

var app = builder.Build();

// Cultura de Honduras para fechas, números y moneda (antes en <globalization> del Web.config).
var cultura = new CultureInfo("es-HN");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

// Mientras dure la migración, /Content y /Scripts se leen de la UI vieja (una sola copia).
// Al retirar la UI vieja: mover esas carpetas a wwwroot y borrar este bloque.
var rutaLegado = Path.GetFullPath(Path.Combine(
    app.Environment.ContentRootPath,
    app.Configuration["RecursosLegado"] ?? "../GESTION_COLEGIAL.UI"));
RutasApp.Configurar(app.Environment.ContentRootPath, rutaLegado);
if (Directory.Exists(rutaLegado))
{
    app.Environment.WebRootFileProvider = new CompositeFileProvider(
        app.Environment.WebRootFileProvider,
        new CarpetaConPrefijoFileProvider("/Content", Path.Combine(rutaLegado, "Content")),
        new CarpetaConPrefijoFileProvider("/Scripts", Path.Combine(rutaLegado, "Scripts")));
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/account/error");
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Mismas rutas que RouteConfig.cs de MVC 5.
app.MapControllerRoute(
    name: "ControllerIndex",
    pattern: "{controller}",
    defaults: new { action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
