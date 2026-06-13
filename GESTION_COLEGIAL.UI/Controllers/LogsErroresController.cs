using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Logs de Errores")]
    public class LogsErroresController : BaseController
    {
        private static readonly string LogPath =
            System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Logs/") ?? @"C:\Logs\";

        public ActionResult Index() => View();

        public ActionResult ListAsync()
        {
            var logs = new List<object>();
            if (Directory.Exists(LogPath))
            {
                foreach (var f in Directory.GetFiles(LogPath, "*.log").OrderByDescending(x => x))
                {
                    var info = new FileInfo(f);
                    logs.Add(new
                    {
                        Archivo           = info.Name,
                        Tamano            = (info.Length / 1024.0).ToString("F1") + " KB",
                        FechaModificacion = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                    });
                }
            }
            return Json(logs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeerAsync(string archivo)
        {
            if (string.IsNullOrWhiteSpace(archivo) || archivo.Contains(".."))
                return Json(new { contenido = "Archivo no válido." }, JsonRequestBehavior.AllowGet);

            var path = Path.Combine(LogPath, Path.GetFileName(archivo));
            if (!System.IO.File.Exists(path))
                return Json(new { contenido = "Archivo no encontrado." }, JsonRequestBehavior.AllowGet);

            // Leer las últimas 200 líneas
            var lines   = System.IO.File.ReadAllLines(path);
            var contenido = string.Join("\n", lines.Skip(Math.Max(0, lines.Length - 200)));
            return Json(new { contenido }, JsonRequestBehavior.AllowGet);
        }
    }
}
