using GESTION_COLEGIAL.UI.Filters;
using System.Collections.Generic;
using System.Web;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Importador / Exportador")]
    public class ImportadorController : BaseController
    {
        public ActionResult Index() => View();

        [HttpPost]
        public ActionResult PreviewAsync(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return Json(new { success = false, message = "No se seleccionó archivo." });

            var rows = new List<object>();
            using (var reader = new System.IO.StreamReader(archivo.OpenReadStream()))
            {
                string headerLine = reader.ReadLine();
                if (headerLine == null)
                    return Json(new { success = false, message = "Archivo vacío." });

                var headers = headerLine.Split(',');
                int count   = 0;

                while (!reader.EndOfStream && count < 10)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');
                    var row    = new Dictionary<string, string>();
                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                        row[headers[i].Trim()] = values[i].Trim();

                    rows.Add(row);
                    count++;
                }
            }
            return Json(new { success = true, filas = rows });
        }
    }
}
